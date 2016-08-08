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
    [RoutePrefix("api/question")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QuestionController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Question> _questionRepository;
        private readonly IEntityBaseRepository<ChoiceAnswer> _choiceQuestionRepository;
        private readonly IEntityBaseRepository<MatchingAnswer> _matchQuestionRepository;
        private readonly IEntityBaseRepository<DescriptiveAnswer> _descriptiveQuestionRepository;
        private readonly IEntityBaseRepository<User> _userRepository;
        public QuestionController()
            : base(null, null)
        {

        }

        public QuestionController(
            IEntityBaseRepository<Question> questionRepository,
            IEntityBaseRepository<ChoiceAnswer> choiceQuestionRepository,
            IEntityBaseRepository<MatchingAnswer> matchQuestionRepository,
            IEntityBaseRepository<DescriptiveAnswer> descriptiveQuestionRepository,
            IEntityBaseRepository<User> userRepository,
            IEntityBaseRepository<Error> _errorsRepository,
            IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _questionRepository = questionRepository;
            _choiceQuestionRepository = choiceQuestionRepository;
            _matchQuestionRepository = matchQuestionRepository;
            _descriptiveQuestionRepository = descriptiveQuestionRepository;
            _userRepository = userRepository;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("question/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var question = _questionRepository.GetSingle(id);

                List<Question> questions = new List<Question>();
                questions.Add(question);
                IEnumerable<QuestionViewModel> questionVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);

                foreach (QuestionViewModel qvm in questionVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.sType = CommonMethods.getType(qvm.Type);
                    qvm.sTypeShort = CommonMethods.getTypeShort(qvm.Type);
                }

                response = request.CreateResponse<IEnumerable<QuestionViewModel>>(HttpStatusCode.OK, questionVM);
                return response;
            });
        }


        [AllowAnonymous]
        [Route("gett")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var questions = _questionRepository.GetAll().ToList();

                IEnumerable<QuestionViewModel> questionsVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);

                response = request.CreateResponse<IEnumerable<QuestionViewModel>>(HttpStatusCode.OK, questionsVM);

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
                List<Question> questions = null;
                int totalQuestions = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    questions = _questionRepository.FindBy(c => c.Text.ToLower().Contains(filter) ||
                            c.Hint.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalQuestions = _questionRepository.GetAll()
                        .Where(c => c.Text.ToLower().Contains(filter) ||
                            c.Hint.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    questions = _questionRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalQuestions = _questionRepository.GetAll().Count();
                }
                //Get User Name
                List<string> UserName = new List<string>();
                UserName = GetUser(questions);

                ICollection<QuestionViewModel> questionsVM = Mapper.Map<ICollection<Question>, ICollection<QuestionViewModel>>(questions);

                int i = 0;
                foreach (QuestionViewModel qvm in questionsVM)
                {
                    if (questions[i].Matches != null && questions[i].Matches.Count() > 0)
                        copyMatchingPair(questions[i], qvm);
                    if (questions[i].QuestionPoolMaps != null && questions[i].QuestionPoolMaps.Count() > 0)
                        copyQuestionPool(questions[i], qvm);
                    qvm.UserName = UserName[i];
                    i++;
                }

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



        //Get User Name
        private List<string> GetUser(List<Question> questions)
        {
            List<string> ListNames = new List<string>();
            foreach (var q in questions)
            {

                if (q.LockedBy != 0)
                {
                    User user = new User();
                    user = _userRepository.GetSingle(q.LockedBy);
                    ListNames.Add(user.Username);
                }
                else
                {
                    ListNames.Add("");
                }

            }
            return ListNames;
        }




        private void copyMatchingPair(Question q, QuestionViewModel qvm)
        {
            if (q.Matches != null)
            {
                if (qvm.Matches == null)
                    qvm.Matches = new List<AnswerMatchPairViewModel>();

                foreach (MatchingAnswer m in q.Matches)
                {
                    AnswerMatchPairViewModel t = new AnswerMatchPairViewModel();
                    t.ChoiceA = m.ChoiceA;
                    t.ChoiceB = m.ChoiceB;
                    t.ChoiceId = m.ChoiceId;
                    t.QuestionId = m.QuestionId;
                    qvm.Matches.Add(t);
                }
            }
        }

        private void copyQuestionPool(Question q, QuestionViewModel qvm)
        {
            if (q.QuestionPoolMaps != null)
            {
                if (qvm.PoolQuestionMap == null)
                    qvm.PoolQuestionMap = new List<PoolQuestionViewModel>();

                foreach (PoolQuestionMapping m in q.QuestionPoolMaps)
                {
                    PoolQuestionViewModel t = new PoolQuestionViewModel();
                    t.PoolID = m.PoolId;
                    t.QuestionID = m.QuestionId;
                    t.IsMandatory = m.IsMandatory;
                    qvm.PoolQuestionMap.Add(t);
                }
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("filterquestions/{topicid:int=-1}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? topicid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Question> questions = null;
                int totalQuestions = new int();

                questions = _questionRepository.GetAll()
                    .Where(q => q.TopicId == topicid)
                    .OrderBy(c => c.ID)
                .ToList();

                totalQuestions = _questionRepository.GetAll().Count();

                IEnumerable<QuestionViewModel> questionsVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);

                PaginationSet<QuestionViewModel> pagedSet = new PaginationSet<QuestionViewModel>()
                {
                    TotalCount = totalQuestions,
                    Items = questionsVM
                };

                response = request.CreateResponse<PaginationSet<QuestionViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }



        //Search Question New
        [AllowAnonymous]
        [HttpGet]
        [Route("searchquestions/{stdid:int=-1}/{subid:int=-1}/{topicid:int=-1}/{qtype:int=-1}/{qstatus:int=-1}/{qdifficulty:int=-1}")] //{qcode}/{question}/
        public HttpResponseMessage SearchQuestion(HttpRequestMessage request, int? page, int? pageSize, string code, string text, int? type, int? status, int? difficultyLevel, int? topicid, int? subjectid, int? standardid, int? qPoolId)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Question> questions = null;
                int totalQuestions = new int();

                questions = _questionRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (text != null ? q.Text.Contains(text) : 1 == 1) &&
                                (type != -1 ? q.Type == type : 1 == 1) &&
                                (topicid != -1 ? q.TopicId == topicid : 1 == 1) &&
                                (subjectid != -1 ? q.Topic.Mapping.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.Topic.Mapping.StandardId == standardid : 1 == 1) &&
                                (status != -1 ? q.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalQuestions = _questionRepository.GetAll()
               .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                           (text != null ? q.Text.Contains(text) : 1 == 1) &&
                           (type != -1 ? q.Type == type : 1 == 1) &&
                           (topicid != -1 ? q.TopicId == topicid : 1 == 1) &&
                           (subjectid != -1 ? q.Topic.Mapping.SubjectId == subjectid : 1 == 1) &&
                           (standardid != -1 ? q.Topic.Mapping.StandardId == standardid : 1 == 1) &&
                           (status != -1 ? q.Status == status : 1 == 1) &&
                           (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

               )
               .Count();


                IEnumerable<QuestionViewModel> questionsVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);
                int i = 0;
                foreach (QuestionViewModel qvm in questionsVM)
                {
                    if (questions[i].Matches != null && questions[i].Matches.Count() > 0)
                        copyMatchingPair(questions[i], qvm);
                    if (questions[i].QuestionPoolMaps != null && questions[i].QuestionPoolMaps.Count() > 0)
                        copyQuestionPool(questions[i], qvm);
                    i++;
                }

                foreach (QuestionViewModel qvm in questionsVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.sType = CommonMethods.getType(qvm.Type);
                    qvm.sTypeShort = CommonMethods.getTypeShort(qvm.Type);
                }

                foreach (QuestionViewModel q in questionsVM)
                {
                    if (q.PoolQuestionMap != null)
                    {
                        foreach (PoolQuestionViewModel m in q.PoolQuestionMap)
                        {
                            if (m.PoolID == qPoolId)
                            {
                                q.IsMappedToPool = true;
                                break;
                            }
                        }
                    }
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
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, QuestionViewModel questionVM)
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
                    Question question = new Question();

                    question.Code = questionVM.Code;
                    question.Text = questionVM.Text;
                    question.Objective = questionVM.Objective;
                    question.Hint = questionVM.Hint;
                    question.TopicId = questionVM.TopicID;
                    question.Type = questionVM.Type;
                    question.DifficultyLevel = questionVM.DifficultyLevel;
                    question.Points = questionVM.Points;
                    question.Status = questionVM.Status;
                    question.Randomize = questionVM.Randomize;
                    question.IsActive = true;
                    question.OnCreated = DateTime.Now;
                    question.OnLocked = DateTime.Now;
                    question.OnUpdated = DateTime.Now;

                    var questions = _questionRepository.FindBy(c => c.Code.ToLower().Contains(question.Code.ToLower())).ToList();

                    if (questions != null && questions.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        //Add Question type
                        AddQuestionType(question, questionVM);

                        _questionRepository.Add(question);
                        _unitOfWork.Commit();

                        // Update view model
                        questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                        response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.Created, questionVM);
                    }
                }
                return response;
            });
        }

        private void AddQuestionType(Question question, QuestionViewModel questionVM)
        {
            if (question.Type == 1)
            {
                AnswerDescriptiveViewModel desc = new AnswerDescriptiveViewModel();
                DescriptiveAnswer ans = new DescriptiveAnswer();
                if (questionVM.Descriptive != null)
                {
                    if (questionVM.Descriptive.Count() > 0)
                    {
                        ans.Keywords = questionVM.Descriptive[0].Keywords;
                        ans.NegativePoints = questionVM.Descriptive[0].NegativePoints;
                        ans.PositivePoints = questionVM.Descriptive[0].PositivePoints;
                        ans.IsAnswer = questionVM.Descriptive[0].IsAnswer;
                        question.Descriptive.Add(ans);
                    }
                }
            }
            else if (question.Type == 2)
            {
                foreach (AnswerChoiceViewModel v in questionVM.Choices)
                {
                    ChoiceAnswer ans = new ChoiceAnswer();
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    question.Choices.Add(ans);
                }
            }
            else if (question.Type == 3)
            {
                foreach (AnswerMatchPairViewModel v in questionVM.Matches)
                {
                    MatchingAnswer ans = new MatchingAnswer();
                    ans.ChoiceA = v.ChoiceA;
                    ans.ChoiceB = v.ChoiceB;
                    ans.ChoiceId = v.ChoiceId;
                    question.Matches.Add(ans);
                }
            }
            else if (question.Type == 4 || question.Type == 5)
            {
                foreach (AnswerChoiceViewModel v in questionVM.Choices)
                {
                    ChoiceAnswer ans = new ChoiceAnswer();
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;

                    question.Choices.Add(ans);
                }
            }
            else if (question.Type == 6)
            {
                AnswerChoiceViewModel choice = new AnswerChoiceViewModel();

            }


        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, QuestionViewModel questionVM)
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
                    Question question = new Question();

                    question = _questionRepository.GetSingle(questionVM.ID);
                    var questions = _questionRepository.FindBy(c => c.Code.ToLower().Contains(questionVM.Code.ToLower())).ToList();
                    if (question.Code != questionVM.Code && questions != null && questions.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        if (question != null)
                        {
                            question.Code = questionVM.Code;
                            question.Text = questionVM.Text;
                            question.Objective = questionVM.Objective;
                            question.Hint = questionVM.Hint;
                            question.TopicId = questionVM.TopicID;
                            question.Type = questionVM.Type;
                            question.DifficultyLevel = questionVM.DifficultyLevel;
                            question.Points = questionVM.Points;
                            question.Status = questionVM.Status;
                            question.Randomize = questionVM.Randomize;
                            question.IsActive = true;
                            question.IsLock = questionVM.IsLock;
                            question.LockedBy = questionVM.LockedBy;
                            question.OnUpdated = DateTime.Now;

                            //Update Question Type
                            UpdateQuestionType(question, questionVM);

                            _questionRepository.Edit(question);
                            _unitOfWork.Commit();

                            // Update view model
                            questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                            response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.Created, questionVM);
                        }
                    }
                }

                return response;
            });
        }

        private void UpdateQuestionType(Question question, QuestionViewModel questionVM)
        {
            if (question.Type == 1)
            {
                DescriptiveAnswer ans = new DescriptiveAnswer();
                if (questionVM.Descriptive != null)
                {
                    if (questionVM.Descriptive.Count() > 0)
                    {
                        if (question.Descriptive.Count() > 0)
                        {
                            {
                                var item = question.Descriptive.Last();
                                question.Descriptive.Remove(item);
                                _descriptiveQuestionRepository.Delete(item);
                            }

                            {

                                ans.Keywords = questionVM.Descriptive[0].Keywords;
                                ans.NegativePoints = questionVM.Descriptive[0].NegativePoints;
                                ans.PositivePoints = questionVM.Descriptive[0].PositivePoints;
                                ans.IsAnswer = questionVM.Descriptive[0].IsAnswer;
                                question.Descriptive.Add(ans);

                            }
                        }
                    }
                }
            }
            else if (question.Type == 2)
            {
                if (question.Choices.Count() > questionVM.Choices.Count())
                {
                    while (question.Choices.Count() != questionVM.Choices.Count())
                    {
                        var item = question.Choices.Last();
                        question.Choices.Remove(item);
                        _choiceQuestionRepository.Delete(item);
                    }
                }
                if (question.Choices.Count() < questionVM.Choices.Count())
                {
                    while (question.Choices.Count() != questionVM.Choices.Count())
                    {
                        ChoiceAnswer ans = new ChoiceAnswer();
                        question.Choices.Add(ans);
                    }
                }
                int j = 0;
                foreach (ChoiceAnswer ans in question.Choices)
                {
                    AnswerChoiceViewModel v = questionVM.Choices[j];
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = j; // v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    j++;
                }
            }
            else if (question.Type == 3)
            {
                if (question.Matches.Count() > questionVM.Matches.Count())
                {
                    while (question.Matches.Count() != questionVM.Matches.Count())
                    {
                        var item = question.Matches.Last();
                        question.Matches.Remove(item);
                        _matchQuestionRepository.Delete(item);
                    }
                }
                if (question.Matches.Count() < questionVM.Matches.Count())
                {
                    while (question.Matches.Count() != questionVM.Matches.Count())
                    {
                        MatchingAnswer ans = new MatchingAnswer();
                        question.Matches.Add(ans);
                    }
                }
                int j = 0;
                foreach (MatchingAnswer ans in question.Matches)
                {
                    AnswerMatchPairViewModel v = questionVM.Matches[j];
                    ans.ChoiceA = v.ChoiceA;
                    ans.ChoiceB = v.ChoiceB;
                    ans.ChoiceId = j;
                    j++;
                }
            }
            else if (question.Type == 4 || question.Type == 5)
            {
                if (question.Choices.Count() > questionVM.Choices.Count())
                {
                    while (question.Choices.Count() != questionVM.Choices.Count())
                    {
                        var item = question.Choices.Last();
                        question.Choices.Remove(item);
                        _choiceQuestionRepository.Delete(item);
                    }
                }
                if (question.Choices.Count() < questionVM.Choices.Count())
                {
                    while (question.Choices.Count() != questionVM.Choices.Count())
                    {
                        ChoiceAnswer ans = new ChoiceAnswer();
                        question.Choices.Add(ans);
                    }
                }
                int j = 0;
                foreach (ChoiceAnswer ans in question.Choices)
                {
                    AnswerChoiceViewModel v = questionVM.Choices[j];
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = j; // v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    j++;
                }
            }
            else if (question.Type == 6)
            {
                AnswerChoiceViewModel choice = new AnswerChoiceViewModel();

            }

        }


        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, int? page, int? pageSize, string code, string text, int? type, int? status, int? difficultyLevel, int? topicid, int? subjectid, int? standardid)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Question> questions = null;
                int totalQuestions = new int();

                questions = _questionRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (text != null ? q.Text.Contains(text) : 1 == 1) &&
                                (type != -1 ? q.Type == type : 1 == 1) &&
                                (topicid != -1 ? q.TopicId == topicid : 1 == 1) &&
                                (subjectid != -1 ? q.Topic.Mapping.SubjectId == subjectid : 1 == 1) &&
                                (standardid != -1 ? q.Topic.Mapping.StandardId == standardid : 1 == 1) &&
                                (status != -1 ? q.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalQuestions = questions.Count();

                if (totalQuestions > 0)
                {
                    List<string> UserName = new List<string>();
                    UserName = GetUser(questions);

                    IEnumerable<QuestionViewModel> questionsVM = Mapper.Map<IEnumerable<Question>, IEnumerable<QuestionViewModel>>(questions);
                    int i = 0;
                    foreach (QuestionViewModel qvm in questionsVM)
                    {
                        qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                        qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                        qvm.sType = CommonMethods.getType(qvm.Type);
                        qvm.sTypeShort = CommonMethods.getTypeShort(qvm.Type);
                        qvm.UserName = UserName[i];
                        i++;
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
                else {
                    response = request.CreateResponse(HttpStatusCode.NoContent, "No Record Found");
                }
                return response;
            });
        }


        //Lock UnLock Questions

        [HttpPost]
        [Route("lockQuestion")]
        public HttpResponseMessage LockQuestion(HttpRequestMessage request, QuestionViewModel questionVM)
        {
            return CommonLockUnLockQuestion(request, questionVM, true);

        }


        [HttpPost]
        [Route("unLockQuestion")]
        public HttpResponseMessage UnLockQuestion(HttpRequestMessage request, QuestionViewModel questionVM)
        {
            return CommonLockUnLockQuestion(request, questionVM, false);

        }



        private HttpResponseMessage CommonLockUnLockQuestion(HttpRequestMessage request, QuestionViewModel questionVM, bool isLock)
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
                    Question question = new Question();
                    question = _questionRepository.GetSingle(questionVM.ID);

                    if (isLock)
                    {
                        if (question.IsLock == 0) //For Lock
                        {
                            question.IsLock = 1;
                            question.LockedBy = questionVM.LockedBy;
                            question.OnLocked = DateTime.Now;
                            _questionRepository.Edit(question);
                            _unitOfWork.Commit();
                            // Update view model
                            questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                            response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.Created, questionVM);

                        }
                        else
                        {
                            // Update view model
                            questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                            response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.NotAcceptable, questionVM);
                        }
                    }
                    else
                    {
                        if (question.IsLock == 1)   //For Unlock
                        {
                            question.IsLock = 0;
                            question.LockedBy = questionVM.LockedBy;
                            question.OnLocked = DateTime.Now;
                            _questionRepository.Edit(question);
                            _unitOfWork.Commit();

                            // Update view model
                            questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                            response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.Created, questionVM);

                        }
                        else
                        {
                            // Update view model
                            questionVM = Mapper.Map<Question, QuestionViewModel>(question);
                            response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.NotAcceptable, questionVM);
                        }
                    }
                }
                return response;
            });
        }

        [HttpPost]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, List<int> questionid)
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
                    foreach (var id in questionid)
                    {
                        Question deleteQuestion = _questionRepository.GetSingle(id);
                        if (deleteQuestion != null)
                        {
                            _questionRepository.Delete(deleteQuestion);
                        }
                    }
                    _unitOfWork.Commit();
                    response = request.CreateResponse<QuestionViewModel>(HttpStatusCode.OK, null);
                }
                return response;
            });
        }
    }
}
