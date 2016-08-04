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
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Institute.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/pools")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PoolsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Pool> _poolRepository;
        private readonly IEntityBaseRepository<PoolQuestionMapping> _poolQuestionRepository;
        private readonly IEntityBaseRepository<Question> _questionRepository;

        private readonly IEntityBaseRepository<User> _userRepository;

        public PoolsController(
            IEntityBaseRepository<Pool> poolRepository, 
            IEntityBaseRepository<PoolQuestionMapping> poolQuestionRepository, 
            IEntityBaseRepository<Question> questionRepository,
            IEntityBaseRepository<User> userRepository,
            IEntityBaseRepository<Error> _errorsRepository, 
            IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _poolRepository = poolRepository;
            _poolQuestionRepository = poolQuestionRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("pool/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var pool = _poolRepository.GetSingle(id);
                List<Pool> pools = new List<Pool>();
                pools.Add(pool);
                IEnumerable<PoolViewModel> topicVM = Mapper.Map<IEnumerable<Pool>, IEnumerable<PoolViewModel>>(pools);
                response = request.CreateResponse<IEnumerable<PoolViewModel>>(HttpStatusCode.OK, topicVM);
                return response;
            });
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var mappings = _poolRepository.GetAll().ToList();

                IEnumerable<PoolViewModel> poolVM = Mapper.Map<IEnumerable<Pool>, IEnumerable<PoolViewModel>>(mappings);

                response = request.CreateResponse<IEnumerable<PoolViewModel>>(HttpStatusCode.OK, poolVM);

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
                List<Pool> pools = null;
                int totalTests = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    pools = _poolRepository.FindBy(c => c.Name.ToLower().Contains(filter) 
                        //||
                        //    c.Objective.ToLower().Contains(filter) ||
                        //    c.Code.ToLower().Contains(filter))
                        ).OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalTests = _poolRepository.GetAll()
                        .Where(c => c.Name.ToLower().Contains(filter) 
                            //||
                            //c.Objective.ToLower().Contains(filter) ||
                            //c.Code.ToLower().Contains(filter))
                        ).Count();
                }
                else
                {
                    pools = _poolRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalTests = _poolRepository.GetAll().Count();
                }


                //Get User Name
                List<string> UserName = new List<string>();
                UserName = GetUser(pools);

                IEnumerable<PoolViewModel> poolsVM = Mapper.Map<IEnumerable<Pool>, IEnumerable<PoolViewModel>>(pools);
                int i = 0;
                foreach (PoolViewModel qvm in poolsVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.UserName = UserName[i];
                    i++;
                }

                PaginationSet<PoolViewModel> pagedSet = new PaginationSet<PoolViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalTests,
                    TotalPages = (int)Math.Ceiling((decimal)totalTests / currentPageSize),
                    Items = poolsVM
                };

                response = request.CreateResponse<PaginationSet<PoolViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        //Get User Name
        private List<string> GetUser(List<Pool> pools)
        {
            List<string> ListNames = new List<string>();
            foreach (var t in pools)
            {

                if (t.LockedBy != 0)
                {
                    User user = new User();
                    user = _userRepository.GetSingle(t.LockedBy);
                    ListNames.Add(user.Username);
                }
                else
                {

                    ListNames.Add("");
                }

            }
            return ListNames;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("questionpoolcnt/{poolid:int=0}")]
        public HttpResponseMessage QuestionsInPoolCount(HttpRequestMessage request, int? poolid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<PoolQuestionMapping> pools = null;
                int questionCnt = new int();

                if (poolid != null)
                {
                    pools = _poolQuestionRepository.FindBy(c => c.PoolId == poolid).OrderBy(c => c.ID)
                        .ToList();

                    questionCnt = pools.Count();
                }
                else
                {
                    questionCnt = 0;
                }
                response = request.CreateResponse<int>(HttpStatusCode.OK, questionCnt);
                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("questionpool/{poolid:int=0}")]
        public HttpResponseMessage QuestionsInPool(HttpRequestMessage request, int? page, int? pageSize, int? poolid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Question> questions = null;
                int totalQuestions = new int();
                ICollection<QuestionViewModel> questionsVM = null;

                if (poolid != null)
                {
                    questions = _poolQuestionRepository.FindBy(c => c.PoolId == poolid)
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .Select(s => s.Question)
                        .ToList();

                    totalQuestions = _poolQuestionRepository.GetAll()
                      .Where(c => c.PoolId == poolid)
                      .Count();


                    questionsVM = Mapper.Map<ICollection<Question>, ICollection<QuestionViewModel>>(questions);
                    foreach (QuestionViewModel qvm in questionsVM)
                    {
                        qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                        qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                        qvm.sType = CommonMethods.getType(qvm.Type);
                        qvm.sTypeShort = CommonMethods.getTypeShort(qvm.Type);
                    }
                    PaginationSet<QuestionViewModel> pagedSet = new PaginationSet<QuestionViewModel>()
                    {
                        Page = currentPage,
                        TotalCount = totalQuestions,
                        TotalPages = (int)Math.Ceiling((decimal)totalQuestions / currentPageSize),
                        Items = questionsVM
                    };

                    response = request.CreateResponse<PaginationSet<QuestionViewModel>>(HttpStatusCode.OK, pagedSet);
                }
                else 
                {
                    response = request.CreateResponse<ICollection<QuestionViewModel>>(HttpStatusCode.BadRequest, questionsVM);
                }
                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("filtertests")] //{stdid:int=-1}/{subid:int=-1}
        public HttpResponseMessage FilteredTests(HttpRequestMessage request, int? testid) //, int? stdid, int? subid
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Pool> pools = null;
                int totalTests = new int();

                pools = _poolRepository.GetAll()
                    .Where(c => ((testid == -1 ? true : c.TestID == testid)))
                    .OrderBy(c => c.ID)
                    .ToList();

                totalTests = _poolRepository.GetAll().Count();
                //Get User Name
                List<string> UserName = new List<string>();
                UserName = GetUser(pools);

                IEnumerable<PoolViewModel> poolsVM = Mapper.Map<IEnumerable<Pool>, IEnumerable<PoolViewModel>>(pools);
                int i = 0;
                foreach (PoolViewModel qvm in poolsVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.UserName = UserName[i];
                    i++;
                }

                PaginationSet<PoolViewModel> pagedSet = new PaginationSet<PoolViewModel>()
                {
                    TotalCount = totalTests,
                    Items = poolsVM
                };

                response = request.CreateResponse<PaginationSet<PoolViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, PoolViewModel poolVM)
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
                    Pool pool = new Pool();

                    pool.TestID = poolVM.TestID;
                    pool.Name = poolVM.Name;
                    pool.Status = poolVM.Status;
                    pool.IsMandatoryToPass = poolVM.IsMandatoryToPass;
                    pool.NoOfQuestionsOutOf = poolVM.NoOfQuestionsOutOf;
                    pool.PassingScore = poolVM.PassingScore;
                    pool.NegativeMarks = poolVM.NegativeMarks;
                    pool.PoolTotalMarks = poolVM.PoolTotalMarks;
                    pool.RandomizeChoice = poolVM.RandomizeChoice;
                    pool.RandomizeQuestion = poolVM.RandomizeQuestion;
                    pool.DifficultyLevel = poolVM.DifficultyLevel;
                    pool.OnLocked = DateTime.Now;
                    pool.OnCreated = DateTime.Now;
                    pool.OnUpdated = DateTime.Now;
                    _poolRepository.Add(pool);
                    _unitOfWork.Commit();

                    // Update view model
                    poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                    response = request.CreateResponse<PoolViewModel>(HttpStatusCode.Created, poolVM);
                }
                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, PoolViewModel poolVM)
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
                    Pool pool = new Pool();

                    pool = _poolRepository.GetSingle(poolVM.ID);

                    pool.TestID = poolVM.TestID;
                    pool.Name = poolVM.Name;
                    pool.Status = poolVM.Status;
                    pool.IsMandatoryToPass = poolVM.IsMandatoryToPass;
                    pool.NoOfQuestionsOutOf = poolVM.NoOfQuestionsOutOf;
                    pool.PassingScore = poolVM.PassingScore;
                    pool.NegativeMarks = poolVM.NegativeMarks;
                    pool.PoolTotalMarks = poolVM.PoolTotalMarks;
                    pool.RandomizeChoice = poolVM.RandomizeChoice;
                    pool.RandomizeQuestion = poolVM.RandomizeQuestion;
                    pool.DifficultyLevel = poolVM.DifficultyLevel;
                    pool.OnUpdated = DateTime.Now;
                    _poolRepository.Edit(pool);
                    _unitOfWork.Commit();

                    // Update view model
                    poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                    response = request.CreateResponse<PoolViewModel>(HttpStatusCode.Created, poolVM);
                }


                return response;
            });
        }


        // Lock and Unlock
        [HttpPost]
        [Route("lockTest")]
        public HttpResponseMessage LockTest(HttpRequestMessage request, PoolViewModel poolVM)
        {
            return CommonLockUnLockTest(request, poolVM, true);

        }


        [HttpPost]
        [Route("unLockTest")]
        public HttpResponseMessage UnLockTest(HttpRequestMessage request, PoolViewModel poolVM)
        {
            return CommonLockUnLockTest(request, poolVM, false);

        }



        private HttpResponseMessage CommonLockUnLockTest(HttpRequestMessage request, PoolViewModel poolVM, bool isLock)
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
                    Pool pool = new Pool();

                    pool = _poolRepository.GetSingle(poolVM.ID);

                    if (isLock)
                    {
                        if (pool.IsLock == 0) //For Lock
                        {
                            pool.IsLock = 1;
                            pool.LockedBy = poolVM.LockedBy;
                            pool.OnLocked = DateTime.Now;
                            _poolRepository.Edit(pool);
                            _unitOfWork.Commit();


                            // Update view model
                            poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                            response = request.CreateResponse<PoolViewModel>(HttpStatusCode.Created, poolVM);

                        }
                        else
                        {
                            // Update view model
                            poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                            response = request.CreateResponse<PoolViewModel>(HttpStatusCode.NotAcceptable, poolVM);


                        }


                    }
                    else
                    {
                        if (pool.IsLock == 1)   //For Unlock
                        {
                            pool.IsLock = 0;
                            pool.LockedBy = poolVM.LockedBy;
                            pool.OnLocked = DateTime.Now;
                            _poolRepository.Edit(pool);
                            _unitOfWork.Commit();


                            // Update view model
                            poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                            response = request.CreateResponse<PoolViewModel>(HttpStatusCode.Created, poolVM);

                        }
                        else
                        {
                            // Update view model
                            poolVM = Mapper.Map<Pool, PoolViewModel>(pool);
                            response = request.CreateResponse<PoolViewModel>(HttpStatusCode.NotAcceptable, poolVM);
                        }
                    }
                }
                return response;
            });
        }


        //Pool Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? testid, string name, int? page, int? pageSize, int? status, int? difficultyLevel)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Pool> pools = null;
                int totalPools = new int();

                pools = _poolRepository.GetAll()
                    .Where(q => (testid != null ? q.TestID == testid : 1 == 1) &&
                                (name != null ? q.Name.Contains(name) : 1 == 1) &&
                                (status != -1 ? q.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalPools = _poolRepository.GetAll()
                       .Where(q => (testid != null ? q.TestID == testid : 1 == 1) &&
                                (name != null ? q.Name.Contains(name) : 1 == 1) &&
                                (status != -1 ? q.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

                        )
                        .Count();
                //Get User Name
                List<string> UserName = new List<string>();
                UserName = GetUser(pools);

                IEnumerable<PoolViewModel> poolsVM = Mapper.Map<IEnumerable<Pool>, IEnumerable<PoolViewModel>>(pools);
                int i = 0;
                foreach (PoolViewModel pvm in poolsVM)
                {
                    pvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(pvm.DifficultyLevel);
                    pvm.sStatus = CommonMethods.getStatus(pvm.Status);
                    pvm.UserName = UserName[i];
                    i++;
                }

                PaginationSet<PoolViewModel> pagedSet = new PaginationSet<PoolViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalPools,
                    TotalPages = (int)Math.Ceiling((decimal)totalPools / currentPageSize),
                    Items = poolsVM
                };

                response = request.CreateResponse<PaginationSet<PoolViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        //Pool Question Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("questionadvancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string text, int? type, int? status, int? difficultyLevel, int? topicid, int? subjectid, int? standardid, int? poolid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Question> questions = null;
                int totalQuestions = new int();

                questions = _poolQuestionRepository.FindBy(c => c.PoolId == poolid)
                    .Where(q => (code != null ? q.Question.Code.Contains(code) : 1 == 1) &&
                                (text != null ? q.Question.Text.Contains(text) : 1 == 1) &&
                                (type != -1 ? q.Question.Type == type : 1 == 1) &&
                                (topicid != -1 ? q.Question.TopicId == topicid : 1 == 1) &&
                                (subjectid != -1 ? q.Question.Topic.Mapping.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.Question.Topic.Mapping.StandardId == standardid : 1 == 1) &&
                                (status != -1 ? q.Question.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.Question.DifficultyLevel == difficultyLevel : 1 == 1) 
                                

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .Select(s => s.Question)
                    .ToList();


                totalQuestions = _poolQuestionRepository.FindBy(c => c.PoolId == poolid)
                    .Where(q => (code != null ? q.Question.Code.Contains(code) : 1 == 1) &&
                                (text != null ? q.Question.Text.Contains(text) : 1 == 1) &&
                                (type != -1 ? q.Question.Type == type : 1 == 1) &&
                                (topicid != -1 ? q.Question.TopicId == topicid : 1 == 1) &&
                                (subjectid != -1 ? q.Question.Topic.Mapping.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.Question.Topic.Mapping.StandardId == standardid : 1 == 1) &&
                                (status != -1 ? q.Question.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.Question.DifficultyLevel == difficultyLevel : 1 == 1)


                    )
                .Count();
                

                IEnumerable<QuestionViewModel> questionsVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);
                
                foreach (QuestionViewModel qvm in questionsVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.sType = CommonMethods.getType(qvm.Type);
                    qvm.sTypeShort = CommonMethods.getTypeShort(qvm.Type);
                
                }

                PaginationSet<QuestionViewModel> pagedSet = new PaginationSet<QuestionViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalQuestions,
                    TotalPages = (int)Math.Ceiling((decimal)totalQuestions / currentPageSize),
                    Items = questionsVM
                };

                response = request.CreateResponse<PaginationSet<QuestionViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        [HttpPost]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, List<int> poolid)
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
                    bool flag = false;
                    foreach (var id in poolid)
                    {
                        Pool deletePool = _poolRepository.GetSingle(id);
                        if (deletePool.PoolQuestionMapping.Count() > 0)
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest,
                               "Can not delete because pool is associated with " + deletePool.PoolQuestionMapping.Count() + " quetions");
                            flag = true;
                            break;
                        }
                        else
                        {
                            _poolRepository.Delete(deletePool);
                        }
                    }
                    if (!flag)
                    {
                        _unitOfWork.Commit();
                        response = request.CreateResponse<PoolViewModel>(HttpStatusCode.OK, null);
                    }
                }
                return response;
            });
        }

        [HttpPost]
        [Route("addquestiontopool")]
        public HttpResponseMessage AddQuestion2Pool(HttpRequestMessage request, List<PoolQuestionViewModel> questions)
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
                    //List<PoolQuestionMapping> pools = new List<PoolQuestionMapping>();

                    foreach (var q in questions)
                    {
                        PoolQuestionMapping pool = new PoolQuestionMapping();
                        pool.PoolId = q.PoolID;
                        pool.QuestionId = q.QuestionID;
                        pool.IsMandatory = q.IsMandatory;

                        _poolQuestionRepository.Add(pool);
                    }
                    _unitOfWork.Commit();

                    response = request.CreateResponse<PoolQuestionViewModel>(HttpStatusCode.Created, null);
                }
                return response;
            });
        }

        [HttpPost]
        [Route("removequestionpoolmap")]
        public HttpResponseMessage RemoveQuestionFromPool(HttpRequestMessage request, List<PoolQuestionViewModel> questions)
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
                    foreach (var q in questions)
                    {
                        PoolQuestionMapping pool = 
                        _poolQuestionRepository.FindBy(x => x.PoolId == q.PoolID && x.QuestionId == q.QuestionID).Single();
                        if (pool != null)
                            _poolQuestionRepository.Delete(pool);
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse<PoolQuestionViewModel>(HttpStatusCode.Created, null);
                }
                return response;
            });
        }
    }
}
