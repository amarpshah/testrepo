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
    [RoutePrefix("api/subjects")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SubjectsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Subject> _subjectsRepository;

        public SubjectsController(IEntityBaseRepository<Subject> subjectsRepository,
             IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _subjectsRepository = subjectsRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var subjects = _subjectsRepository.GetAll().ToList();

                IEnumerable<SubjectViewModel> subjectsVM = Mapper.Map<IEnumerable<Subject>, IEnumerable<SubjectViewModel>>(subjects);

                response = request.CreateResponse<IEnumerable<SubjectViewModel>>(HttpStatusCode.OK, subjectsVM);

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
                List<Subject> subjects = null;
                int totalSubjects = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    subjects = _subjectsRepository.FindBy(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalSubjects = _subjectsRepository.GetAll()
                        .Where(c => c.Code.ToLower().Contains(filter) ||
                            c.Name.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    subjects = _subjectsRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalSubjects = _subjectsRepository.GetAll().Count();
                }

                IEnumerable<SubjectViewModel> subjectsVM = Mapper.Map<IEnumerable<Subject>, IEnumerable<SubjectViewModel>>(subjects);

                PaginationSet<SubjectViewModel> pagedSet = new PaginationSet<SubjectViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalSubjects,
                    TotalPages = (int)Math.Ceiling((decimal)totalSubjects / currentPageSize),
                    Items = subjectsVM
                };

                response = request.CreateResponse<PaginationSet<SubjectViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string subject)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Subject> subjects = null;
                int totalSubjects = new int();

                subjects = _subjectsRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (subject != null ? q.Name.Contains(subject) : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalSubjects = _subjectsRepository.GetAll()
                .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                            (subject != null ? q.Name.Contains(subject) : 1 == 1)


                )
                .Count();

                IEnumerable<SubjectViewModel> subjectVM = Mapper.Map<IEnumerable<Subject>, IEnumerable<SubjectViewModel>>(subjects);



                PaginationSet<SubjectViewModel> pagedSet = new PaginationSet<SubjectViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalSubjects,
                    TotalPages = (int)Math.Ceiling((decimal)totalSubjects / currentPageSize),
                    Items = subjectVM
                };

                response = request.CreateResponse<PaginationSet<SubjectViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("filtersubject/{stdid:int=-1}")]
        public HttpResponseMessage FilterSubject(HttpRequestMessage request, int? stdid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Subject> subjects = null;
                int totalSubjects = new int();

                subjects = _subjectsRepository.GetAll()
                    .Where(c => c.StandardMapping.Where(s => s.StandardId == (int)stdid).Any())
                    .OrderBy(c => c.ID)
                .ToList();

                totalSubjects = _subjectsRepository.GetAll().Count();

                IEnumerable<SubjectViewModel> subjectsVM = Mapper.Map<IEnumerable<Subject>, IEnumerable<SubjectViewModel>>(subjects);

                PaginationSet<SubjectViewModel> pagedSet = new PaginationSet<SubjectViewModel>()
                {
                    TotalCount = totalSubjects,
                    Items = subjectsVM
                };

                response = request.CreateResponse<PaginationSet<SubjectViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getstandardlist/{subid:int=-1}")]
        public HttpResponseMessage GetStandardList(HttpRequestMessage request, int? subid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Subject> subjects = null;
                int totalSubjects = new int();

                subjects = _subjectsRepository.GetAll()
                    .Where(c => c.ID == subid)
                    //.Where(c => c.StandardMapping.Where(s => s.SubjectId == (int)subid).Any())
                    .OrderBy(c => c.ID)
                .ToList();

                totalSubjects = _subjectsRepository.GetAll().Count();

                List<Subject2ViewModel> subjectsVM = new List<Subject2ViewModel>();
                if (subjects.Count > 0)
                {
                    //IEnumerable<Subject2ViewModel> subjectsVM = Mapper.Map<IEnumerable<Subject>, IEnumerable<Subject2ViewModel>>(subjects);
                    foreach (var s in subjects[0].StandardMapping)
                    {
                        subjectsVM.Add(new Subject2ViewModel { Standard = s.Standard.Name });
                    }
                }

                PaginationSet<Subject2ViewModel> pagedSet = new PaginationSet<Subject2ViewModel>()
                {
                    TotalCount = totalSubjects,
                    Items = subjectsVM
                };

                response = request.CreateResponse<PaginationSet<Subject2ViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, SubjectViewModel subject)
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
                    Subject newSubject = new Subject();

                    newSubject.Code = subject.Code;
                    newSubject.Name = subject.Subject;

                    var subjects = _subjectsRepository.FindBy(c => c.Code.ToLower().Contains(newSubject.Code.ToLower())).ToList();
                    if (subjects != null && subjects.Count > 0)
                    {

                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {

                        _subjectsRepository.Add(newSubject);

                        _unitOfWork.Commit();

                        // Update view model
                        subject = Mapper.Map<Subject, SubjectViewModel>(newSubject);
                        response = request.CreateResponse<SubjectViewModel>(HttpStatusCode.Created, subject);
                    }
                }

                return response;
            });
        }


        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, SubjectViewModel subjectVM)
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
                    Subject subject = new Subject();

                    subject = _subjectsRepository.GetSingle(subjectVM.ID);
                    var subjects = _subjectsRepository.FindBy(c => c.Code.ToLower().Contains(subjectVM.Code.ToLower())).ToList();
                    if (subject.Code != subjectVM.Code && subjects != null && subjects.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        subject.Code = subjectVM.Code;
                        subject.Name = subjectVM.Subject;
                        _subjectsRepository.Edit(subject);
                        _unitOfWork.Commit();

                        // Update view model
                        subjectVM = Mapper.Map<Subject, SubjectViewModel>(subject);
                        response = request.CreateResponse<SubjectViewModel>(HttpStatusCode.Created, subjectVM);
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
                    Subject deleteSub = _subjectsRepository.GetSingle(id);
                    if (deleteSub.StandardMapping.Count() > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Can not delete because subject is associated with " + deleteSub.StandardMapping.Count() + " standards");
                    }
                    else
                    {
                        _subjectsRepository.Delete(deleteSub);
                        _unitOfWork.Commit();

                        // Update view model
                        SubjectViewModel subjectsVM = Mapper.Map<Subject, SubjectViewModel>(deleteSub);
                        response = request.CreateResponse<SubjectViewModel>(HttpStatusCode.OK, subjectsVM);
                    }
                }
                return response;
            });
        }

    }
}
