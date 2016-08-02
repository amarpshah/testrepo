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
    //[Authorize(Roles = "Admin")]
    [RoutePrefix("api/standards")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StandardsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Standard> _standardsRepository;

        public StandardsController(IEntityBaseRepository<Standard> standardsRepository,
             IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _standardsRepository = standardsRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var standards = _standardsRepository.GetAll().ToList();

                IEnumerable<StandardViewModel> standardsVM = Mapper.Map<IEnumerable<Standard>, IEnumerable<StandardViewModel>>(standards);

                response = request.CreateResponse<IEnumerable<StandardViewModel>>(HttpStatusCode.OK, standardsVM);

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
                List<Standard> standard = null;
                int totalStandards = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    standard = _standardsRepository.FindBy(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalStandards = _standardsRepository.GetAll()
                        .Where(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    standard = _standardsRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalStandards = _standardsRepository.GetAll().Count();
                }

                IEnumerable<StandardViewModel> standardVM = Mapper.Map<IEnumerable<Standard>, IEnumerable<StandardViewModel>>(standard);

                PaginationSet<StandardViewModel> pagedSet = new PaginationSet<StandardViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalStandards,
                    TotalPages = (int)Math.Ceiling((decimal)totalStandards / currentPageSize),
                    Items = standardVM
                };

                response = request.CreateResponse<PaginationSet<StandardViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string standard, string division)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Standard> standards = null;
                int totalStandards = new int();

                standards = _standardsRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (standard != null ? q.Name.Contains(standard) : 1 == 1) &&
                                (division != null ? q.Division.Contains(division) : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalStandards = _standardsRepository.GetAll()
                .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                            (standard != null ? q.Name.Contains(standard) : 1 == 1) &&
                            (division != null ? q.Division.Contains(division) : 1 == 1)

                )
                .Count();

                IEnumerable<StandardViewModel> standardVM = Mapper.Map<IEnumerable<Standard>, IEnumerable<StandardViewModel>>(standards);



                PaginationSet<StandardViewModel> pagedSet = new PaginationSet<StandardViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalStandards,
                    TotalPages = (int)Math.Ceiling((decimal)totalStandards / currentPageSize),
                    Items = standardVM
                };

                response = request.CreateResponse<PaginationSet<StandardViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, StandardViewModel standard)
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
                    Standard newStandard = new Standard();

                    newStandard.Code = standard.Code;
                    newStandard.Name = standard.Standard;
                    newStandard.Division = standard.Division;

                    var standards = _standardsRepository.FindBy(c => c.Code.ToLower().Contains(newStandard.Code.ToLower())).ToList();
                    if (standards != null && standards.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        _standardsRepository.Add(newStandard);
                        _unitOfWork.Commit();

                        // Update view model
                        standard = Mapper.Map<Standard, StandardViewModel>(newStandard);
                        response = request.CreateResponse<StandardViewModel>(HttpStatusCode.Created, standard);
                    }
                }
                return response;
            });
        }


        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, StandardViewModel standardVM)
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
                    Standard standard = new Standard();

                    standard = _standardsRepository.GetSingle(standardVM.ID);
                    var standards = _standardsRepository.FindBy(c => c.Code.ToLower().Contains(standardVM.Code.ToLower())).ToList();
                    if (standard.Code != standardVM.Code && standards != null && standards.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Duplicate code cannot be inserted");

                    }
                    else
                    {
                        standard.Code = standardVM.Code;
                        standard.Name = standardVM.Standard;
                        standard.Division = standardVM.Division;

                        _standardsRepository.Edit(standard);
                        _unitOfWork.Commit();

                        // Update view model
                        standardVM = Mapper.Map<Standard, StandardViewModel>(standard);
                        response = request.CreateResponse<StandardViewModel>(HttpStatusCode.Created, standardVM);
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

                    Standard deleteStd = _standardsRepository.GetSingle(id);

                    if (deleteStd.SubjectMapping.Count() > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Can not delete because standard is associated with " + deleteStd.SubjectMapping.Count() + " subjects");
                    }
                    else
                    {
                        _standardsRepository.Delete(deleteStd);

                        _unitOfWork.Commit();

                        // Update view model
                        response = request.CreateResponse<StandardViewModel>(HttpStatusCode.Created, new StandardViewModel());
                    }
                }

                return response;
            });
        }

    }
}
