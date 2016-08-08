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
    [RoutePrefix("api/topic")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TopicController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Topic> _topicRepository;

        public TopicController(
            IEntityBaseRepository<Topic> topicRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _topicRepository = topicRepository;
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var mappings = _topicRepository.GetAll().ToList();

                IEnumerable<TopicViewModel> topicVM = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicViewModel>>(mappings);

                response = request.CreateResponse<IEnumerable<TopicViewModel>>(HttpStatusCode.OK, topicVM);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("topic/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var topic = _topicRepository.GetSingle(id);
                List<Topic> topics = new List<Topic>();
                topics.Add(topic);
                IEnumerable<TopicViewModel> topicVM = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicViewModel>>(topics);
                response = request.CreateResponse<IEnumerable<TopicViewModel>>(HttpStatusCode.OK, topicVM);
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
                List<Topic> topics = null;
                int totalTopics = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    topics = _topicRepository.FindBy(c => c.Name.ToLower().Contains(filter) ||
                            c.Objective.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalTopics = _topicRepository.GetAll()
                        .Where(c => c.Name.ToLower().Contains(filter) ||
                            c.Objective.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    topics = _topicRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalTopics = _topicRepository.GetAll().Count();
                }

                IEnumerable<TopicViewModel> topicsVM = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicViewModel>>(topics);

                PaginationSet<TopicViewModel> pagedSet = new PaginationSet<TopicViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalTopics,
                    TotalPages = (int)Math.Ceiling((decimal)totalTopics / currentPageSize),
                    Items = topicsVM
                };

                response = request.CreateResponse<PaginationSet<TopicViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string name, int? subjectid, int? standardid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Topic> topics = null;
                int totaltopics = new int();

                topics = _topicRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (name != null ? q.Name.Contains(name) : 1 == 1) &&
                                (subjectid != -1 ? q.Mapping.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.Mapping.StandardId == standardid : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totaltopics = topics.Count();
                if (totaltopics > 0)
                {
                    IEnumerable<TopicViewModel> topicsVM = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicViewModel>>(topics);
                    PaginationSet<TopicViewModel> pagedSet = new PaginationSet<TopicViewModel>()
                    {
                        Page = currentPage,
                        TotalCount = totaltopics,
                        TotalPages = (int)Math.Ceiling((decimal)totaltopics / currentPageSize),
                        Items = topicsVM
                    };

                    response = request.CreateResponse<PaginationSet<TopicViewModel>>(HttpStatusCode.OK, pagedSet);
                }
                else {
                    response = request.CreateResponse(HttpStatusCode.NoContent, "No Record Found");
                }
                return response;
            });
        }





        [AllowAnonymous]
        [HttpGet]
        [Route("filtertopics/{stdid:int=-1}/{subid:int=-1}")]
        public HttpResponseMessage FilteredTopics(HttpRequestMessage request, int? mappingid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Topic> topics = null;
                int totalTopics = new int();

                topics = _topicRepository.GetAll()
                    .Where(c => (mappingid == -1 ? true : c.MappingID == mappingid))
                                 .OrderBy(c => c.ID)
                                 .ToList();

                totalTopics = _topicRepository.GetAll().Count();

                IEnumerable<TopicViewModel> topicsVM = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicViewModel>>(topics);

                PaginationSet<TopicViewModel> pagedSet = new PaginationSet<TopicViewModel>()
                {
                    TotalCount = totalTopics,
                    Items = topicsVM
                };

                response = request.CreateResponse<PaginationSet<TopicViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, TopicViewModel topicVM)
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
                    Topic topic = new Topic();

                    topic.Code = topicVM.Code;
                    topic.Name = topicVM.Name;
                    topic.Objective = topicVM.Objective;
                    topic.MappingID = topicVM.MappingID;
                    topic.IsActive = true;

                    var topics = _topicRepository.FindBy(c => c.Code.ToLower().Contains(topic.Code.ToLower())).ToList();

                    if (topics != null && topics.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {

                        _topicRepository.Add(topic);
                        _unitOfWork.Commit();

                        // Update view model
                        topicVM = Mapper.Map<Topic, TopicViewModel>(topic);
                        response = request.CreateResponse<TopicViewModel>(HttpStatusCode.Created, topicVM);
                    }


                }

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, TopicViewModel topicVM)
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
                    Topic topic = new Topic();

                    topic = _topicRepository.GetSingle(topicVM.ID);
                    var topics = _topicRepository.FindBy(c => c.Code.ToLower().Contains(topicVM.Code.ToLower())).ToList();

                    if (topic.Code != topicVM.Code && topics != null && topics.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        topic.Code = topicVM.Code;
                        topic.Name = topicVM.Name;
                        topic.Objective = topicVM.Objective;
                        topic.MappingID = topicVM.MappingID;
                        topic.IsActive = true;

                        _topicRepository.Edit(topic);
                        _unitOfWork.Commit();

                        // Update view model
                        topicVM = Mapper.Map<Topic, TopicViewModel>(topic);
                        response = request.CreateResponse<TopicViewModel>(HttpStatusCode.Created, topicVM);
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
                    Topic deleteTopic = _topicRepository.GetSingle(id);

                    if (deleteTopic.Questions.Count() > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Can not delete because topic is associated with " + deleteTopic.Questions.Count() + " questions");
                    }
                    else
                    {

                        _topicRepository.Delete(deleteTopic);
                        _unitOfWork.Commit();

                        // Update view model
                        TopicViewModel topicVM = Mapper.Map<Topic, TopicViewModel>(deleteTopic);
                        response = request.CreateResponse<TopicViewModel>(HttpStatusCode.OK, topicVM);
                    }
                }
                return response;
            });
        }
    }
}
