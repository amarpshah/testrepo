using AutoMapper;
using Institute.BizComponent.Infrastructure;
using Institute.BizComponent.Repositories;
using Institute.Entities;
using Institute.WebApi.Infrastructure.Core;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Institute.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/mapping")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MappingController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<StandardSubjectMapping> _mappingsRepository;

        public MappingController(
            IEntityBaseRepository<StandardSubjectMapping> mappingsRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _mappingsRepository = mappingsRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var mappings = _mappingsRepository.GetAll().ToList();

                IEnumerable<MappingViewModel> mappingsVM = Mapper.Map<IEnumerable<StandardSubjectMapping>, IEnumerable<MappingViewModel>>(mappings);

                response = request.CreateResponse<IEnumerable<MappingViewModel>>(HttpStatusCode.OK, mappingsVM);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<StandardSubjectMapping> mappings = null;
                int totalMappings = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    mappings = _mappingsRepository.FindBy(c => c.Standard.Name.ToLower().Contains(filter) ||
                            c.Subject.Name.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalMappings = _mappingsRepository.GetAll()
                        .Where(c => c.Standard.Name.ToLower().Contains(filter) ||
                            c.Subject.Name.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    mappings = _mappingsRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalMappings = _mappingsRepository.GetAll().Count();
                }

                IEnumerable<MappingViewModel> mappingsVM = Mapper.Map<IEnumerable<StandardSubjectMapping>, IEnumerable<MappingViewModel>>(mappings);

                PaginationSet<MappingViewModel> pagedSet = new PaginationSet<MappingViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMappings,
                    TotalPages = (int)Math.Ceiling((decimal)totalMappings / currentPageSize),
                    Items = mappingsVM
                };

                response = request.CreateResponse<PaginationSet<MappingViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, int? standardid, int? subjectid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<StandardSubjectMapping> mappings = null;
                int totalMappings = new int();

                mappings = _mappingsRepository.GetAll()
                    .Where(q => (subjectid != -1 ? q.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.StandardId == standardid : 1 == 1)
                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalMappings = mappings.Count();
                if (totalMappings > 0)
                {

                    IEnumerable<MappingViewModel> ssMappingVM = Mapper.Map<IEnumerable<StandardSubjectMapping>, IEnumerable<MappingViewModel>>(mappings);

                    PaginationSet<MappingViewModel> pagedSet = new PaginationSet<MappingViewModel>()
                    {
                        Page = currentPage,
                        TotalCount = totalMappings,
                        TotalPages = (int)Math.Ceiling((decimal)totalMappings / currentPageSize),
                        Items = ssMappingVM
                    };

                    response = request.CreateResponse<PaginationSet<MappingViewModel>>(HttpStatusCode.OK, pagedSet);
                }
                else {
                    response = request.CreateResponse(HttpStatusCode.NoContent, "No Record Found");                
                }
                return response;
            });
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("filtermappings/{stdid:int=-1}/{subid:int=-1}")]
        public HttpResponseMessage FilterMapping(HttpRequestMessage request, int? stdid, int? subid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<StandardSubjectMapping> mappings = null;
                int totalMappings = new int();

                mappings = _mappingsRepository.GetAll()
                    .Where(m => ((stdid == -1 ? true : m.StandardId == stdid)
                        && (subid == -1 ? true : m.SubjectId == subid)))
                    .OrderBy(c => c.ID)
                .ToList();

                totalMappings = _mappingsRepository.GetAll().Count();

                IEnumerable<MappingViewModel> mappingsVM = Mapper.Map<IEnumerable<StandardSubjectMapping>, IEnumerable<MappingViewModel>>(mappings);

                PaginationSet<MappingViewModel> pagedSet = new PaginationSet<MappingViewModel>()
                {
                    TotalCount = totalMappings,
                    Items = mappingsVM
                };

                response = request.CreateResponse<PaginationSet<MappingViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, IEnumerable<MappingViewModel> mappingList)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    if (mappingList.Count() > 0)
                    {
                        List<StandardSubjectMapping> listMaps = new List<StandardSubjectMapping>();
                        foreach (MappingViewModel map in mappingList)
                        {
                            StandardSubjectMapping mapModel = new StandardSubjectMapping();

                            mapModel.StandardId = map.StandardID;
                            mapModel.SubjectId = map.SubjectID;
                            mapModel.IsActive = true;
                            listMaps.Add(mapModel);
                            _mappingsRepository.Add(mapModel);
                        }
                        _unitOfWork.Commit();

                        // Update view model
                        mappingList = Mapper.Map<IEnumerable<StandardSubjectMapping>, IEnumerable<MappingViewModel>>(listMaps);
                        response = request.CreateResponse<IEnumerable<MappingViewModel>>(HttpStatusCode.Created, mappingList);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                             "Select Standard and Subject");
                    }
                }
                return response;
            });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    StandardSubjectMapping deleteMapping = _mappingsRepository.GetSingle(id);

                    if (deleteMapping.Topic.Count() > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Can not delete because Mapping is associated with " + deleteMapping.Topic.Count() + " topics");
                    }
                    else
                    {
                        _mappingsRepository.Delete(deleteMapping);
                        _unitOfWork.Commit();

                        // Update view model
                        MappingViewModel subjectsVM = Mapper.Map<StandardSubjectMapping, MappingViewModel>(deleteMapping);
                        response = request.CreateResponse<MappingViewModel>(HttpStatusCode.OK, subjectsVM);
                    }
                }
                return response;
            });
        }
    }
}
