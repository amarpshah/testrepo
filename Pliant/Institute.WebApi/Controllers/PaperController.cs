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
using Institute.WebApi.Controllers;

namespace Institute.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/papers")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PaperController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Pool> _poolRepository;
        private readonly IEntityBaseRepository<PoolQuestionMapping> _poolQuestionRepository;
        private readonly IEntityBaseRepository<Paper> _paperRepository;
        private readonly IEntityBaseRepository<TempTestQuestion> _tempTestQuestionRepository;
        private readonly IEntityBaseRepository<TestQuestion> _testQuestionRepository;
        private readonly IEntityBaseRepository<TempChoiceAnswer> _tempChoiceQuestionRepository;
        private readonly IEntityBaseRepository<TempDescriptiveAnswer> _tempDescriptiveQuestionRepository;
        private readonly IEntityBaseRepository<TempMatchingAnswer> _tempMatchQuestionRepository;
        private readonly IEntityBaseRepository<FinalChoiceAnswer> _finalChoiceQuestionRepository;
        private readonly IEntityBaseRepository<FinalDescriptiveAnswer> _finalDescriptiveQuestionRepository;
        private readonly IEntityBaseRepository<FinalMatchingAnswer> _finalMatchQuestionRepository;

        public PaperController(
            IEntityBaseRepository<Pool> poolRepository,
            IEntityBaseRepository<PoolQuestionMapping> poolQuestionRepository,
            IEntityBaseRepository<Paper> paperRepository,
            IEntityBaseRepository<TempTestQuestion> tempTestQuestionRepository,
            IEntityBaseRepository<TestQuestion> testQuestionRepository,
            IEntityBaseRepository<TempChoiceAnswer> tempChoiceQuestionRepository,
            IEntityBaseRepository<TempDescriptiveAnswer> tempDescriptiveQuestionRepository,
            IEntityBaseRepository<TempMatchingAnswer> tempMatchQuestionRepository,
            IEntityBaseRepository<FinalChoiceAnswer> finalChoiceQuestionRepository,
            IEntityBaseRepository<FinalDescriptiveAnswer> finalDescriptiveQuestionRepository,
            IEntityBaseRepository<FinalMatchingAnswer> finalMatchQuestionRepository,
            IEntityBaseRepository<Error> _errorsRepository,
           IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _poolRepository = poolRepository;
            _poolQuestionRepository = poolQuestionRepository;
            _paperRepository = paperRepository;
            _tempTestQuestionRepository = tempTestQuestionRepository;
            _testQuestionRepository = testQuestionRepository;
            _tempChoiceQuestionRepository = tempChoiceQuestionRepository;
            _tempDescriptiveQuestionRepository = tempDescriptiveQuestionRepository;
            _tempMatchQuestionRepository = tempMatchQuestionRepository;
            _finalChoiceQuestionRepository = finalChoiceQuestionRepository;
            _finalDescriptiveQuestionRepository = finalDescriptiveQuestionRepository;
            _finalMatchQuestionRepository = finalMatchQuestionRepository;
        }



        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Register(HttpRequestMessage request, PaperViewModel paperVM)
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
                    Paper paper = new Paper();

                    paper.TestID = paperVM.TestID;
                    paper.TestName = paperVM.TestName;
                    paper.NoOfSets = paperVM.NoOfSets;
                    paper.Description = paperVM.Description;
                    paper.CreatedBy = paperVM.CreatedBy;
                    paper.IsFinalized = false;
                    paper.OnCreated = DateTime.Now;


                    var papers = _paperRepository.GetAll()
                               .Where(q => (q.TestID == paper.TestID))
                               .ToList();

                    if (papers != null && papers.Count > 0)
                    {
                        paper = papers[0];
                        paper.NoOfSets = paperVM.NoOfSets;
                        paper.CreatedBy = paperVM.CreatedBy;
                        paper.Description = paperVM.Description;
                        paper.IsFinalized = false;
                        paper.OnCreated = DateTime.Now;
                        _paperRepository.Edit(paper);
                    }
                    else
                    {
                        _paperRepository.Add(paper);

                    }

                    _unitOfWork.Commit();
                 

                    response = Generate(request, paper.ID, paperVM.TestID, paperVM.NoOfSets);
                }
                return response;
            });
        }


        public HttpResponseMessage Generate(HttpRequestMessage request, int paperid, int testid, int totalsets)
        {
            HttpResponseMessage response = null;
            List<Pool> pools = new List<Pool>();
            List<Question> questions = new List<Question>();
            ICollection<TempTestQuestionViewModel> testQuestionVM = null;

            if (testid != null)
            {
                DeleteTempTestQuestion(testid);
                pools = _poolRepository.GetAll()
                                .Where(q => (q.TestID == testid))
                                .ToList();

                for (int i = 0; i < totalsets; i++)
                {
                    foreach (var pool in pools)
                    {
                        int count = 0;
                        List<Question> randomQuestions = new List<Question>();
                        if (pool.ID != null)
                        {
                            questions = _poolQuestionRepository.FindBy(c => c.PoolId == pool.ID)
                                .OrderBy(c => c.ID)
                                .Select(s => s.Question)
                                .ToList();

                            Random r = new Random();
                            var results = questions.OrderBy(item => r.Next());
                            foreach (var result in results)
                            {
                                randomQuestions.Add(result);
                            }

                            foreach (var question in randomQuestions)
                            {
                                TempTestQuestion tempTestQuestion = new TempTestQuestion();

                                count++;

                                tempTestQuestion.TestID = pool.TestID;
                                tempTestQuestion.TestName = pool.Test.Text;
                                tempTestQuestion.PoolID = pool.ID;
                                tempTestQuestion.PoolName = pool.Name;
                                tempTestQuestion.QuestionID = question.ID;
                                tempTestQuestion.QuestionType = question.Type;
                                tempTestQuestion.QuestionText = question.Text;
                                tempTestQuestion.PaperID = paperid;
                                tempTestQuestion.TestSetNo = i+1;
                                tempTestQuestion.SequenceNo = count;

                             

                             if (question.Type == 1)
                                {
                                    TempDescriptiveAnswer ans = new TempDescriptiveAnswer();
                                    if (question.Descriptive != null)
                                    {
                                        if (question.Descriptive.Count() > 0)
                                        {
                                            ans.Keywords = question.Descriptive.ElementAt(0).Keywords; ;
                                            ans.NegativePoints = question.Descriptive.ElementAt(0).NegativePoints;
                                            ans.PositivePoints = question.Descriptive.ElementAt(0).PositivePoints;
                                            ans.IsAnswer = question.Descriptive.ElementAt(0).IsAnswer;
                                            ans.QuestionId = question.ID;
                                            ans.PaperID = paperid;
                                            tempTestQuestion.Descriptive.Add(ans);
                                        }
                                    }
                                }
                                else if (question.Type == 2)
                                {
                                    foreach (ChoiceAnswer v in question.Choices)
                                    {
                                        TempChoiceAnswer ans = new TempChoiceAnswer();
                                        ans.DisplayType = v.DisplayType;
                                        ans.ChoiceId = v.ChoiceId;
                                        ans.Text = v.Text;
                                        ans.PointsPerChoice = v.PointsPerChoice;
                                        ans.IsAnswer = v.IsAnswer;
                                        ans.QuestionId = question.ID;
                                        ans.PaperID = paperid;
                                        tempTestQuestion.Choices.Add(ans);
                                    }
                                }
                                else if (question.Type == 3)
                                {
                                    var index = 0;
                                    var match = question.Matches;
                                    var MatchDisplay = match.OrderBy(item => r.Next()).ToList();
                                    foreach (MatchingAnswer v in question.Matches)
                                    {
                                        TempMatchingAnswer ans = new TempMatchingAnswer();
                                        ans.DisplayB = MatchDisplay[index].ChoiceB;
                                        ans.ChoiceA = v.ChoiceA;
                                        ans.ChoiceB = v.ChoiceB;
                                        ans.ChoiceId = v.ChoiceId;
                                        ans.QuestionId = question.ID;
                                        ans.PaperID = paperid;
                                        tempTestQuestion.Matches.Add(ans);
                                        index++;
                                    }
                                }
                                else if (question.Type == 4 || question.Type == 5)
                                {
                                    foreach (ChoiceAnswer v in question.Choices)
                                    {
                                        TempChoiceAnswer ans = new TempChoiceAnswer();
                                        ans.DisplayType = v.DisplayType;
                                        ans.ChoiceId = v.ChoiceId;
                                        ans.Text = v.Text;
                                        ans.PointsPerChoice = v.PointsPerChoice;
                                        ans.IsAnswer = v.IsAnswer;
                                        ans.QuestionId = question.ID;
                                        ans.PaperID = paperid;
                                        tempTestQuestion.Choices.Add(ans);
                                    }
                                }
                                _tempTestQuestionRepository.Add(tempTestQuestion);
                            }
                        }
                    }
                }
                _unitOfWork.Commit();
                response = request.CreateResponse<ICollection<TempTestQuestionViewModel>>(HttpStatusCode.Created, null);

            }
            else
            {
                response = request.CreateResponse<ICollection<TempTestQuestionViewModel>>(HttpStatusCode.BadRequest, testQuestionVM);
            }

            return response;
        }


       

        // function To get Test Paper info
        [AllowAnonymous]
        [HttpGet]
        [Route("filtertests")]
        public HttpResponseMessage FilteredTests(HttpRequestMessage request, int? testid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Paper> papers = null;

                papers = _paperRepository.GetAll()
                    .Where(c => ((testid == -1 ? true : c.TestID == testid)))
                    .OrderBy(c => c.ID)
                    .ToList();

                IEnumerable<PaperViewModel> paperVM = Mapper.Map<IEnumerable<Paper>, IEnumerable<PaperViewModel>>(papers);
                response = request.CreateResponse<IEnumerable<PaperViewModel>>(HttpStatusCode.OK, paperVM);
                return response;
            });
        }

       
        //// function To get TestPaper
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("filterpaper")]
        //public HttpResponseMessage FilteredPaper(HttpRequestMessage request, int? paperid)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        HttpResponseMessage response = null;
        //        List<TempTestQuestion> tempTestQuestions = null;
        //        List<TestQuestion> testQuestions = null;  //TODO: Check ISFinalized
        //        List<TestSetsViewModel> testSets = new List<TestSetsViewModel>();
        //        bool IsFinalised  = false;
               
        //        var finalPaper = _paperRepository.GetAll()
        //                      .Where(q => (q.ID == paperid))
        //                      .ToList();

        //        if (finalPaper != null && finalPaper.Count > 0)
        //        {
        //            Paper paper = new Paper();
        //            paper = finalPaper[0];
        //            IsFinalised = paper.IsFinalized;
        //        }

        //        if (IsFinalised)
        //        {
        //            testQuestions = _testQuestionRepository.GetAll()
        //                .Where(c => ((c.PaperID == paperid)))    //removed paperid == -1 ? true :
        //                .OrderBy(c => c.ID)
        //                .ToList();

        //            Dictionary<int, string> temp = new Dictionary<int, string>();
        //            Dictionary<int, string> tempPools = new Dictionary<int, string>();
        //            foreach (var t in testQuestions)
        //            {
        //                if (!temp.ContainsKey(t.TestSetNo))
        //                {
        //                    temp.Add(t.TestSetNo, "");
        //                    testSets.Add(new TestSetsViewModel { TestSetNo = t.TestSetNo, TestSetPools = new List<TestSetPoolViewModel>() });

        //                }
        //                if (!tempPools.ContainsKey(t.PoolID))
        //                {
        //                    tempPools.Add(t.PoolID, t.PoolName);

        //                }
        //            }


        //            // Populate Pools inside testSets  
        //            foreach (var ts in testSets)
        //            {
        //                // Populating Questions inside Pools
        //                foreach (var t in tempPools)
        //                {
        //                    TestSetPoolViewModel poolObj = new TestSetPoolViewModel { PoolID = t.Key, PoolName = t.Value, TestSetQuestions = new List<TestSetPoolQuestionViewModel>() };
                            
        //                    foreach (var q in testQuestions)
        //                    {
        //                        var data = CopyTestSetQuestion(q);
        //                        if (q.TestSetNo == ts.TestSetNo && q.PoolID == t.Key)
        //                        {

        //                            poolObj.TestSetQuestions.Add(data);
        //                        }
        //                    }
        //                    ts.TestSetPools.Add(poolObj);
        //                }
        //            }


        //            response = request.CreateResponse<IEnumerable<TestSetsViewModel>>(HttpStatusCode.OK, testSets);

        //        }
        //        else
        //        {
        //            tempTestQuestions = _tempTestQuestionRepository.GetAll()
        //           .Where(c => ((c.PaperID == paperid)))    //removed paperid == -1 ? true :
        //           .OrderBy(c => c.ID)
        //           .ToList();
        //            Dictionary<int, string> temp = new Dictionary<int, string>();
        //            Dictionary<int, string> tempPools = new Dictionary<int, string>();
        //            foreach (var t in tempTestQuestions)
        //            {
        //                if (!temp.ContainsKey(t.TestSetNo))
        //                {
        //                    temp.Add(t.TestSetNo, "");
        //                    testSets.Add(new TestSetsViewModel { TestSetNo = t.TestSetNo, TestSetPools = new List<TestSetPoolViewModel>() });

        //                }
        //                if (!tempPools.ContainsKey(t.PoolID))
        //                {
        //                    tempPools.Add(t.PoolID, t.PoolName);

        //                }
        //            }

        //            foreach (var ts in testSets)
        //            {

        //                foreach (var t in tempPools)
        //                {
        //                    TestSetPoolViewModel poolObj = new TestSetPoolViewModel { PoolID = t.Key, PoolName = t.Value, TestSetQuestions = new List<TestSetPoolQuestionViewModel>() };

        //                    foreach (var q in tempTestQuestions)
        //                    {
        //                        var data = CopyTestSetQuestion(q);
        //                        if (q.TestSetNo == ts.TestSetNo && q.PoolID == t.Key)
        //                        {

        //                            poolObj.TestSetQuestions.Add(data);
        //                        }
        //                    }
        //                    ts.TestSetPools.Add(poolObj);
        //                }
        //            }

        //            //IEnumerable<TempTestQuestionViewModel> tempTestQuestionVM = Mapper.Map<IEnumerable<TempTestQuestion>, IEnumerable<TempTestQuestionViewModel>>(tempTestQuestions);
        //            //response = request.CreateResponse<IEnumerable<TempTestQuestionViewModel>>(HttpStatusCode.OK, tempTestQuestionVM);
        //            response = request.CreateResponse<IEnumerable<TestSetsViewModel>>(HttpStatusCode.OK, testSets);
        //        }
        //        return response;
        //    });
        //}



        private HttpResponseMessage FinalPaper(HttpRequestMessage request, int? paperid)
        {
           
            HttpResponseMessage response = null;
            
            List<TestQuestion> testQuestions = null; 
            List<TestSetsViewModel> testSets = new List<TestSetsViewModel>();

            testQuestions = _testQuestionRepository.GetAll()
                       .Where(c => ((c.PaperID == paperid)))  
                       .OrderBy(c => c.ID)
                       .ToList();

            Dictionary<int, string> temp = new Dictionary<int, string>();
            Dictionary<int, string> tempPools = new Dictionary<int, string>();
            foreach (var t in testQuestions)
            {
                if (!temp.ContainsKey(t.TestSetNo))
                {
                    temp.Add(t.TestSetNo, "");
                    testSets.Add(new TestSetsViewModel { TestSetNo = t.TestSetNo, TestSetPools = new List<TestSetPoolViewModel>() });

                }
                if (!tempPools.ContainsKey(t.PoolID))
                {
                    tempPools.Add(t.PoolID, t.PoolName);

                }
            }


            // Populate Pools inside testSets  
            foreach (var ts in testSets)
            {
                // Populating Questions inside Pools
                foreach (var t in tempPools)
                {
                    TestSetPoolViewModel poolObj = new TestSetPoolViewModel { PoolID = t.Key, PoolName = t.Value, TestSetQuestions = new List<TestSetPoolQuestionViewModel>() };

                    foreach (var q in testQuestions)
                    {
                        var data = CopyTestSetQuestion(q);
                        if (q.TestSetNo == ts.TestSetNo && q.PoolID == t.Key)
                        {

                            poolObj.TestSetQuestions.Add(data);
                        }
                    }
                    ts.TestSetPools.Add(poolObj);
                }
            }


            response = request.CreateResponse<IEnumerable<TestSetsViewModel>>(HttpStatusCode.OK, testSets);
           
            return response;
          
        }

        private HttpResponseMessage TempPaper(HttpRequestMessage request, int? paperid)
        {
            HttpResponseMessage response = null;
            List<TempTestQuestion> tempTestQuestions = null;
            List<TestSetsViewModel> testSets = new List<TestSetsViewModel>();

            tempTestQuestions = _tempTestQuestionRepository.GetAll()
           .Where(c => ((c.PaperID == paperid)))    
           .OrderBy(c => c.ID)
           .ToList();
            Dictionary<int, string> temp = new Dictionary<int, string>();
            Dictionary<int, string> tempPools = new Dictionary<int, string>();
            foreach (var t in tempTestQuestions)
            {
                if (!temp.ContainsKey(t.TestSetNo))
                {
                    temp.Add(t.TestSetNo, "");
                    testSets.Add(new TestSetsViewModel { TestSetNo = t.TestSetNo, TestSetPools = new List<TestSetPoolViewModel>() });

                }
                if (!tempPools.ContainsKey(t.PoolID))
                {
                    tempPools.Add(t.PoolID, t.PoolName);

                }
            }

            foreach (var ts in testSets)
            {

                foreach (var t in tempPools)
                {
                    TestSetPoolViewModel poolObj = new TestSetPoolViewModel { PoolID = t.Key, PoolName = t.Value, TestSetQuestions = new List<TestSetPoolQuestionViewModel>() };

                    foreach (var q in tempTestQuestions)
                    {
                        var data = CopyTestSetQuestion(q);
                        if (q.TestSetNo == ts.TestSetNo && q.PoolID == t.Key)
                        {

                            poolObj.TestSetQuestions.Add(data);
                        }
                    }
                    ts.TestSetPools.Add(poolObj);
                }
            }

            response = request.CreateResponse<IEnumerable<TestSetsViewModel>>(HttpStatusCode.OK, testSets);
            return response;
        }
        
        
        // function To get TestPaper
        [AllowAnonymous]
        [HttpGet]
        [Route("filterpaper")]
        public HttpResponseMessage FilteredPaper(HttpRequestMessage request, int? paperid)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
              
                bool IsFinalised = false;

                var finalPaper = _paperRepository.GetAll()
                              .Where(q => (q.ID == paperid))
                              .ToList();

                if (finalPaper != null && finalPaper.Count > 0)
                {
                    Paper paper = new Paper();
                    paper = finalPaper[0];
                    IsFinalised = paper.IsFinalized;
                }

                if (IsFinalised)
                {
                    response = FinalPaper(request, paperid);
                }
                else
                {
                    response = TempPaper(request, paperid);
                }
                return response;
            });
        }


        private TestSetPoolQuestionViewModel CopyTestSetQuestion(TempTestQuestion tempTestQuestion)
        {
            TestSetPoolQuestionViewModel tsPoolQuestion = new TestSetPoolQuestionViewModel();
            tsPoolQuestion.ID = tempTestQuestion.ID;
            tsPoolQuestion.PaperID = tempTestQuestion.PaperID;
            tsPoolQuestion.PoolID = tempTestQuestion.PoolID;
            tsPoolQuestion.PoolName = tempTestQuestion.PoolName;
            tsPoolQuestion.QuestionID = tempTestQuestion.QuestionID;
            tsPoolQuestion.QuestionType = tempTestQuestion.QuestionType;
            tsPoolQuestion.QuestionText = tempTestQuestion.QuestionText;
            tsPoolQuestion.SequenceNo = tempTestQuestion.SequenceNo;
            tsPoolQuestion.TestID = tempTestQuestion.TestID;
            tsPoolQuestion.TestName = tempTestQuestion.TestName;
            tsPoolQuestion.TestSetNo = tempTestQuestion.TestSetNo;


             if (tempTestQuestion.QuestionType == 2)
            {
                foreach (TempChoiceAnswer v in tempTestQuestion.Choices)
                {
                    TempAnswerChoiceViewModel ans = new TempAnswerChoiceViewModel();
                    ans.ID = v.ID;
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    ans.QuestionId = v.QuestionId;
                    ans.PaperID = tempTestQuestion.PaperID;
                    tsPoolQuestion.Choices.Add(ans);
                }
            }
             else if (tempTestQuestion.QuestionType == 3)
             {
                 foreach (TempMatchingAnswer v in tempTestQuestion.Matches)
                 {
                     TempAnswerMatchPairViewModel ans = new TempAnswerMatchPairViewModel();
                     ans.ID = v.ID;
                     ans.ChoiceA = v.ChoiceA;
                     ans.ChoiceB = v.ChoiceB;
                     ans.DisplayB = v.DisplayB;
                     ans.ChoiceId = v.ChoiceId;
                     ans.QuestionId = v.QuestionId;
                     ans.PaperID = tempTestQuestion.PaperID;
                     ans.TempTestQuestionID = tempTestQuestion.ID;
                     tsPoolQuestion.Matches.Add(ans);
                 }
             }
             else if (tempTestQuestion.QuestionType == 4 || tempTestQuestion.QuestionType == 5)
             {
                 foreach (TempChoiceAnswer v in tempTestQuestion.Choices)
                 {
                     TempAnswerChoiceViewModel ans = new TempAnswerChoiceViewModel();
                     ans.ID = v.ID;
                     ans.DisplayType = v.DisplayType;
                     ans.ChoiceId = v.ChoiceId;
                     ans.Text = v.Text;
                     ans.PointsPerChoice = v.PointsPerChoice;
                     ans.IsAnswer = v.IsAnswer;
                     ans.QuestionId = v.QuestionId;
                     ans.PaperID = tempTestQuestion.PaperID;
                     tsPoolQuestion.Choices.Add(ans);
                 }
             }


            return tsPoolQuestion;
        }
       
        private TestSetPoolQuestionViewModel CopyTestSetQuestion(TestQuestion testQuestion)
        {
            TestSetPoolQuestionViewModel tsPoolQuestion = new TestSetPoolQuestionViewModel();
            tsPoolQuestion.ID = testQuestion.ID;
            tsPoolQuestion.PaperID = testQuestion.PaperID;
            tsPoolQuestion.PoolID = testQuestion.PoolID;
            tsPoolQuestion.PoolName = testQuestion.PoolName;
            tsPoolQuestion.QuestionID = testQuestion.QuestionID;
            tsPoolQuestion.QuestionType = testQuestion.QuestionType;
            tsPoolQuestion.QuestionText = testQuestion.QuestionText;
            tsPoolQuestion.SequenceNo = testQuestion.SequenceNo;
            tsPoolQuestion.TestID = testQuestion.TestID;
            tsPoolQuestion.TestName = testQuestion.TestName;
            tsPoolQuestion.TestSetNo = testQuestion.TestSetNo;

            if (testQuestion.QuestionType == 2)
            {
                foreach (FinalChoiceAnswer v in testQuestion.Choices)
                {
                    TempAnswerChoiceViewModel ans = new TempAnswerChoiceViewModel();
                    ans.ID = v.ID;
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    ans.QuestionId = v.QuestionId;
                    ans.PaperID = testQuestion.PaperID;
                    tsPoolQuestion.Choices.Add(ans);
                }
            }
            else if (testQuestion.QuestionType == 3)
            {
                foreach (FinalMatchingAnswer v in testQuestion.Matches)
                {
                    TempAnswerMatchPairViewModel ans = new TempAnswerMatchPairViewModel();
                    ans.ID = v.ID;
                    ans.ChoiceA = v.ChoiceA;
                    ans.ChoiceB = v.ChoiceB;
                    ans.DisplayB = v.DisplayB;
                    ans.ChoiceId = v.ChoiceId;
                    ans.QuestionId = v.QuestionId;
                    ans.PaperID = testQuestion.PaperID;
                    tsPoolQuestion.Matches.Add(ans);
                }
            }
            else if (testQuestion.QuestionType == 4 || testQuestion.QuestionType == 5)
            {
                foreach (FinalChoiceAnswer v in testQuestion.Choices)
                {
                    TempAnswerChoiceViewModel ans = new TempAnswerChoiceViewModel();
                    ans.ID = v.ID;
                    ans.DisplayType = v.DisplayType;
                    ans.ChoiceId = v.ChoiceId;
                    ans.Text = v.Text;
                    ans.PointsPerChoice = v.PointsPerChoice;
                    ans.IsAnswer = v.IsAnswer;
                    ans.QuestionId = v.QuestionId;
                    ans.PaperID = testQuestion.PaperID;
                    tsPoolQuestion.Choices.Add(ans);
                }
            }


            return tsPoolQuestion;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("finalize")]
        public HttpResponseMessage Finalize(HttpRequestMessage request, int paperid)
        {
            return CreateHttpResponse(request, () =>
           {
               HttpResponseMessage response = null;
               List<TempTestQuestion> tempTestQuestions = null;
               List<TestQuestion> testQuestions = null;
               int testid = 0;
               //TempTestQuestion deleteTempTest = new TempTestQuestion();
               if (paperid != null)
               {
                   tempTestQuestions = _tempTestQuestionRepository.GetAll()
                      .Where(c => ((paperid == -1 ? true : c.PaperID == paperid)))
                      .OrderBy(c => c.ID)
                      .ToList();

                   testQuestions = _testQuestionRepository.GetAll()
                      .Where(c => ((paperid == -1 ? true : c.PaperID == paperid)))
                      .OrderBy(c => c.ID)
                      .ToList();

                   //Delete From Final Tables if Allready Data Exists 
                   //in Final Table
                   if (testQuestions.Count > 0)
                   {

                       foreach (var t in testQuestions)
                       {

                           var finalChoiceAnswers = _finalChoiceQuestionRepository.GetAll().Where(x => x.TestQuestionID == t.ID).ToList();
                           var finalDescriptiveAnswers = _finalDescriptiveQuestionRepository.GetAll().Where(x => x.TestQuestionID == t.ID).ToList();
                           var finalMatchingAnswers = _finalMatchQuestionRepository.GetAll().Where(x => x.TestQuestionID == t.ID).ToList();

                           if (finalChoiceAnswers != null && finalChoiceAnswers.Count() > 0)
                           {
                               foreach (var c in finalChoiceAnswers)
                               {
                                   t.Choices.Remove(c);
                                   _finalChoiceQuestionRepository.Delete(c);
                               }
                           }

                           else if (finalDescriptiveAnswers != null && finalDescriptiveAnswers.Count() > 0)
                           {
                               foreach (var d in finalDescriptiveAnswers)
                               {
                                   t.Descriptive.Remove(d);
                                   _finalDescriptiveQuestionRepository.Delete(d);
                               }
                           }
                           else if (finalMatchingAnswers != null && finalMatchingAnswers.Count() > 0)
                           {
                               foreach (var m in finalMatchingAnswers)
                               {
                                   t.Matches.Remove(m);
                                   _finalMatchQuestionRepository.Delete(m);
                               }
                           }



                           _testQuestionRepository.Delete(t);  //Delete entry from TestQuestion Table
                       }
                   }
                  


                       if (tempTestQuestions.Count > 0)
                       {
                           foreach (var question in tempTestQuestions)
                           {
                               TestQuestion testQuestion = new TestQuestion();

                               testQuestion.PaperID = question.PaperID;
                               testQuestion.PoolID = question.PoolID;
                               testQuestion.PoolName = question.PoolName;
                               testQuestion.TestID = question.TestID;
                               testQuestion.TestName = question.TestName;
                               testQuestion.QuestionID = question.QuestionID;
                               testQuestion.QuestionType = question.QuestionType;
                               testQuestion.QuestionText = question.QuestionText;
                               testQuestion.SequenceNo = question.SequenceNo;
                               testQuestion.TestSetNo = question.TestSetNo;
                               testid = question.TestID; // Later use for Delete Temp Tables
                               if (question.QuestionType == 1)
                               {

                                   FinalDescriptiveAnswer ans = new FinalDescriptiveAnswer();
                                   if (question.Descriptive != null)
                                   {
                                       if (question.Descriptive.Count() > 0)
                                       {
                                           ans.Keywords = question.Descriptive.ElementAt(0).Keywords; ;
                                           ans.NegativePoints = question.Descriptive.ElementAt(0).NegativePoints;
                                           ans.PositivePoints = question.Descriptive.ElementAt(0).PositivePoints;
                                           ans.IsAnswer = question.Descriptive.ElementAt(0).IsAnswer;
                                           ans.QuestionId = question.ID;
                                           ans.PaperID = question.PaperID;

                                           testQuestion.Descriptive.Add(ans);
                                       }
                                   }
                               }
                               else if (question.QuestionType == 2)
                               {
                                   foreach (TempChoiceAnswer v in question.Choices)
                                   {
                                       FinalChoiceAnswer ans = new FinalChoiceAnswer();
                                       ans.DisplayType = v.DisplayType;
                                       ans.ChoiceId = v.ChoiceId;
                                       ans.Text = v.Text;
                                       ans.PointsPerChoice = v.PointsPerChoice;
                                       ans.IsAnswer = v.IsAnswer;
                                       ans.QuestionId = question.ID;
                                       ans.PaperID = paperid;
                                       testQuestion.Choices.Add(ans);
                                   }
                               }
                               else if (question.QuestionType == 3)
                               {
                                   foreach (TempMatchingAnswer v in question.Matches)
                                   {
                                       FinalMatchingAnswer ans = new FinalMatchingAnswer();
                                       ans.ChoiceA = v.ChoiceA;
                                       ans.ChoiceB = v.ChoiceB;
                                       ans.DisplayB = v.DisplayB;
                                       ans.ChoiceId = v.ChoiceId;
                                       ans.QuestionId = question.ID;
                                       ans.PaperID = paperid;
                                       testQuestion.Matches.Add(ans);
                                   }
                               }
                               else if (question.QuestionType == 4 || question.QuestionType == 5)
                               {
                                   foreach (TempChoiceAnswer v in question.Choices)
                                   {
                                       FinalChoiceAnswer ans = new FinalChoiceAnswer();
                                       ans.DisplayType = v.DisplayType;
                                       ans.ChoiceId = v.ChoiceId;
                                       ans.Text = v.Text;
                                       ans.PointsPerChoice = v.PointsPerChoice;
                                       ans.IsAnswer = v.IsAnswer;
                                       ans.QuestionId = question.ID;
                                       ans.PaperID = paperid;
                                       testQuestion.Choices.Add(ans);
                                   }
                               }


                               _testQuestionRepository.Add(testQuestion);
                           }

                           var finalPaper = _paperRepository.GetAll()
                                  .Where(q => (q.ID == paperid))
                                  .ToList();

                           if (finalPaper != null && finalPaper.Count > 0)
                           {
                               Paper paper = new Paper();
                               paper = finalPaper[0];
                               paper.IsFinalized = true;
                               _paperRepository.Edit(paper);
                           }


                          
                           //Delete Record from Temp Test and Temp Answer Tables
                           DeleteTempTestQuestion(testid);
                           
                          
                        
                           _unitOfWork.Commit();
                       }
                       response = request.CreateResponse<TestQuestionViewModel>(HttpStatusCode.Created, null);
                   //}
                   //else if (testQuestions.Count > 0)
                   //{
                   //    response = request.CreateResponse<TestQuestionViewModel>(HttpStatusCode.Created, null);
                   //}
               }
               return response;
           });

        }

        private void DeleteTempTestQuestion(int testid)
        {
            if (testid != null)
            {
                var tempTestQuestions = _tempTestQuestionRepository.GetAll().Where(x => x.TestID == testid).ToList();

                foreach (var t in tempTestQuestions)
                {

                    var tempChoiceAnswers = _tempChoiceQuestionRepository.GetAll().Where(x => x.TempTestQuestionID == t.ID).ToList();
                    var tempDescriptiveAnswers = _tempDescriptiveQuestionRepository.GetAll().Where(x => x.TempTestQuestionID == t.ID).ToList();
                    var tempMatchingAnswers = _tempMatchQuestionRepository.GetAll().Where(x => x.TempTestQuestionID == t.ID).ToList();
                    if (tempChoiceAnswers != null && tempChoiceAnswers.Count() > 0)
                    {
                        foreach (var c in tempChoiceAnswers)
                        {
                            t.Choices.Remove(c);
                            _tempChoiceQuestionRepository.Delete(c);
                        }
                    }

                    else if (tempDescriptiveAnswers != null && tempDescriptiveAnswers.Count() > 0)
                    {
                        foreach (var d in tempDescriptiveAnswers)
                        {
                            t.Descriptive.Remove(d);
                            _tempDescriptiveQuestionRepository.Delete(d);
                        }
                    }
                    else if (tempMatchingAnswers != null && tempMatchingAnswers.Count() > 0)
                    {
                        foreach (var m in tempMatchingAnswers)
                        {
                            t.Matches.Remove(m);
                            _tempMatchQuestionRepository.Delete(m);
                        }
                    }



                    _tempTestQuestionRepository.Delete(t);  //Delete entry from TempTestQuestion Table
                }

            }
        }

    }
}
