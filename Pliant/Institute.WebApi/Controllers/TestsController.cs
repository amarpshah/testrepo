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
    [RoutePrefix("api/tests")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Test> _testRepository;
        private readonly IEntityBaseRepository<User> _userRepository;


        public TestsController(
            IEntityBaseRepository<Test> testRepository,
            IEntityBaseRepository<User> userRepository,
            IEntityBaseRepository<Error> _errorsRepository,

            IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _testRepository = testRepository;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("test/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var mapping = _testRepository.GetSingle(id);
                List<Test> mappings = new List<Test>();
                mappings.Add(mapping);
                IEnumerable<TestViewModel> topicVM = Mapper.Map<IEnumerable<Test>, IEnumerable<TestViewModel>>(mappings);
                response = request.CreateResponse<IEnumerable<TestViewModel>>(HttpStatusCode.OK, topicVM);
                return response;
            });
        }


        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var mappings = _testRepository.GetAll().ToList();

                IEnumerable<TestViewModel> topicVM = Mapper.Map<IEnumerable<Test>, IEnumerable<TestViewModel>>(mappings);

                response = request.CreateResponse<IEnumerable<TestViewModel>>(HttpStatusCode.OK, topicVM);

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
                List<Test> tests = null;
                int totalTests = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    tests = _testRepository.FindBy(c => c.Text.ToLower().Contains(filter) ||
                            c.Objective.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalTests = _testRepository.GetAll()
                        .Where(c => c.Text.ToLower().Contains(filter) ||
                            c.Objective.ToLower().Contains(filter) ||
                            c.Code.ToLower().Contains(filter))
                        .Count();
                }
                else
                {
                    tests = _testRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalTests = _testRepository.GetAll().Count();
                }

                //Get User Name
                List<string> UserName = new List<string>();
                UserName = GetUser(tests);

                IEnumerable<TestViewModel> testsVM = Mapper.Map<IEnumerable<Test>, IEnumerable<TestViewModel>>(tests);
                int i = 0;
                foreach (TestViewModel qvm in testsVM)
                {
                    qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                    qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                    qvm.UserName = UserName[i];
                    i++;
                }

                PaginationSet<TestViewModel> pagedSet = new PaginationSet<TestViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalTests,
                    TotalPages = (int)Math.Ceiling((decimal)totalTests / currentPageSize),
                    Items = testsVM
                };

                response = request.CreateResponse<PaginationSet<TestViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }


        //Get User Name
        private List<string> GetUser(List<Test> tests)
        {
            List<string> ListNames = new List<string>();
            foreach (var t in tests)
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
        [Route("filtertests")] //{stdid:int=-1}/{subid:int=-1}
        public HttpResponseMessage FilteredTests(HttpRequestMessage request) //, int? stdid, int? subid
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Test> tests = null;
                int totalTests = new int();

                tests = _testRepository.GetAll()
                    //                    .Where(c => ((stdid == -1 ? true : c.Mapping.StandardId == stdid) &&
                    //                                 (subid == -1 ? true : c.Mapping.SubjectId == subid)))


                    .OrderBy(c => c.ID)
                .ToList();

                totalTests = _testRepository.GetAll().Count();

                IEnumerable<TestViewModel> testsVM = Mapper.Map<IEnumerable<Test>, IEnumerable<TestViewModel>>(tests);

                PaginationSet<TestViewModel> pagedSet = new PaginationSet<TestViewModel>()
                {
                    TotalCount = totalTests,
                    Items = testsVM
                };

                response = request.CreateResponse<PaginationSet<TestViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, TestViewModel testVM)
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
                    Test test = new Test();

                    test.Code = testVM.Code;
                    test.Text = testVM.Text;
                    test.Description = testVM.Description;
                    test.Objective = testVM.Objective;
                    test.Status = testVM.Status;
                    test.NegativeMarks = testVM.NegativeMarks;
                    test.PointsPerQuestion = testVM.PointPerQuestion;
                    test.TotalMarks = testVM.TotalMarks;
                    test.PassingMarks = testVM.PassingMarks;
                    test.ScoringMode = testVM.ScoringMode;
                    test.DifficultyLevel = testVM.DifficultyLevel;
                    test.PoolSequence = testVM.PoolSequence;
                    test.ShowHint = testVM.ShowHint;
                    test.OnLocked = DateTime.Now;
                    test.OnCreated = DateTime.Now;
                    test.OnUpdated = DateTime.Now;

                    var tests = _testRepository.FindBy(c => c.Code.ToLower().Contains(test.Code.ToLower())).ToList();

                    if (tests != null && tests.Count > 0)
                    {

                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                          "Duplicate code cannot be inserted");
                    }
                    else
                    {
                        _testRepository.Add(test);
                        _unitOfWork.Commit();

                        // Update view model
                        testVM = Mapper.Map<Test, TestViewModel>(test);
                        response = request.CreateResponse<TestViewModel>(HttpStatusCode.Created, testVM);
                    }
                }
                return response;
            });
        }


        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, TestViewModel testVM)
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
                   Test test = new Test();

                   test = _testRepository.GetSingle(testVM.ID);
                   var tests = _testRepository.FindBy(c => c.Code.ToLower().Contains(testVM.Code.ToLower())).ToList();
                   if (test.Code != testVM.Code && tests != null && tests.Count > 0)
                   {
                       response = request.CreateResponse(HttpStatusCode.BadRequest,
                         "Duplicate code cannot be inserted");
                   }
                   else
                   {
                       test.Code = testVM.Code;
                       test.Text = testVM.Text;
                       test.Description = testVM.Description;
                       test.Objective = testVM.Objective;
                       test.Status = testVM.Status;
                       test.NegativeMarks = testVM.NegativeMarks;
                       test.PointsPerQuestion = testVM.PointPerQuestion;
                       test.TotalMarks = testVM.TotalMarks;
                       test.PassingMarks = testVM.PassingMarks;
                       test.ScoringMode = testVM.ScoringMode;
                       test.DifficultyLevel = testVM.DifficultyLevel;
                       test.PoolSequence = testVM.PoolSequence;
                       test.ShowHint = testVM.ShowHint;
                       test.Lock = testVM.Lock;
                       test.LockedBy = testVM.LockedBy;

                       _testRepository.Edit(test);
                       _unitOfWork.Commit();

                       // Update view model
                       testVM = Mapper.Map<Test, TestViewModel>(test);
                       response = request.CreateResponse<TestViewModel>(HttpStatusCode.Created, testVM);
                   }
               }

               return response;
           });
        }

        //Advaced Search

        [AllowAnonymous]
        [HttpGet]
        [Route("advancedsearch")]
        public HttpResponseMessage AdvancedSearch(HttpRequestMessage request, string code, string text, int? page, int? pageSize, int? status, int? difficultyLevel)
        {
            return CreateHttpResponse(request, () =>
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                HttpResponseMessage response = null;
                List<Test> tests = null;
                int totalTests = new int();

                tests = _testRepository.GetAll()
                    .Where(q => (code != null ? q.Code.Contains(code) : 1 == 1) &&
                                (text != null ? q.Text.Contains(text) : 1 == 1) &&
                                (status != -1 ? q.Status == status : 1 == 1) &&
                                (difficultyLevel != -1 ? q.DifficultyLevel == difficultyLevel : 1 == 1)

                    )
                    .OrderBy(c => c.ID)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                totalTests = tests.Count();

                if (totalTests > 0)
                {
                    List<string> UserName = new List<string>();
                    UserName = GetUser(tests);

                    IEnumerable<TestViewModel> testsVM = Mapper.Map<IEnumerable<Test>, IEnumerable<TestViewModel>>(tests);
                    int i = 0;
                    foreach (TestViewModel qvm in testsVM)
                    {
                        qvm.sDifficultyLevel = CommonMethods.getDifficultyLevel(qvm.DifficultyLevel);
                        qvm.sStatus = CommonMethods.getStatus(qvm.Status);
                        qvm.UserName = UserName[i];
                        i++;
                    }

                    PaginationSet<TestViewModel> pagedSet = new PaginationSet<TestViewModel>()
                    {
                        Page = currentPage,
                        TotalCount = totalTests,
                        TotalPages = (int)Math.Ceiling((decimal)totalTests / currentPageSize),
                        Items = testsVM
                    };

                    response = request.CreateResponse<PaginationSet<TestViewModel>>(HttpStatusCode.OK, pagedSet);
                }
                else {
                    response = request.CreateResponse(HttpStatusCode.NoContent, "No Record Found");
                }
                return response;
            });
        }


        // Lock and Unlock
        [HttpPost]
        [Route("lockTest")]
        public HttpResponseMessage LockTest(HttpRequestMessage request, TestViewModel testVM)
        {
            return CommonLockUnLockTest(request, testVM, true);

        }


        [HttpPost]
        [Route("unLockTest")]
        public HttpResponseMessage UnLockTest(HttpRequestMessage request, TestViewModel testVM)
        {
            return CommonLockUnLockTest(request, testVM, false);

        }



        private HttpResponseMessage CommonLockUnLockTest(HttpRequestMessage request, TestViewModel testVM, bool isLock)
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
                   Test test = new Test();

                   test = _testRepository.GetSingle(testVM.ID);

                   if (isLock)
                   {
                       if (test.Lock == 0) //For Lock
                       {
                           test.Lock = 1;
                           test.LockedBy = testVM.LockedBy;
                           test.OnLocked = DateTime.Now;
                           _testRepository.Edit(test);
                           _unitOfWork.Commit();


                           // Update view model
                           testVM = Mapper.Map<Test, TestViewModel>(test);
                           response = request.CreateResponse<TestViewModel>(HttpStatusCode.Created, testVM);

                       }
                       else
                       {
                           // Update view model
                           testVM = Mapper.Map<Test, TestViewModel>(test);
                           response = request.CreateResponse<TestViewModel>(HttpStatusCode.NotAcceptable, testVM);


                       }


                   }
                   else
                   {
                       if (test.Lock == 1)   //For Unlock
                       {
                           test.Lock = 0;
                           test.LockedBy = testVM.LockedBy;
                           test.OnLocked = DateTime.Now;
                           _testRepository.Edit(test);
                           _unitOfWork.Commit();


                           // Update view model
                           testVM = Mapper.Map<Test, TestViewModel>(test);
                           response = request.CreateResponse<TestViewModel>(HttpStatusCode.Created, testVM);

                       }
                       else
                       {
                           // Update view model
                           testVM = Mapper.Map<Test, TestViewModel>(test);
                           response = request.CreateResponse<TestViewModel>(HttpStatusCode.NotAcceptable, testVM);
                       }
                   }
               }
               return response;
           });
        }



        [HttpPost]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, List<int> testid)
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
                    foreach (var id in testid)
                    {
                        Test deleteTest = _testRepository.GetSingle(id);
                        if (deleteTest.Pools.Count() > 0)
                        {
                            response = request.CreateResponse(HttpStatusCode.BadRequest,
                               "Can not delete because test is associated with " + deleteTest.Pools.Count() + " pools");
                            flag = true;
                            break;
                        }
                        else
                        {
                            _testRepository.Delete(deleteTest);
                        }
                    }
                    if (!flag)
                    {
                        _unitOfWork.Commit();
                        response = request.CreateResponse<TestViewModel>(HttpStatusCode.OK, null);
                    }
                }
                return response;
            });
        }
    }
}

