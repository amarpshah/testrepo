using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class TestSetsViewModel
    {
        public int TestSetNo { get; set; }
        public List<TestSetPoolViewModel> TestSetPools { get; set; }
    }

    public class TestSetPoolViewModel 
    {
        public int PoolID { get; set; }
        public string PoolName { get; set; }
        public List<TestSetPoolQuestionViewModel> TestSetQuestions { get; set; }
    
    }
    public class TestSetPoolQuestionViewModel
    {

        public TestSetPoolQuestionViewModel()
        {
            Choices = new List<TempAnswerChoiceViewModel>();
            Descriptive = new List<TempAnswerDescriptiveViewModel>();
            Matches = new List<TempAnswerMatchPairViewModel>();
        }
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public int PoolID { get; set; }
        public string PoolName { get; set; }
        public int QuestionID { get; set; }
        public int QuestionType { get; set; }
        public string QuestionText { get; set; }
        public int PaperID { get; set; }
        public int TestSetNo { get; set; }
        public int SequenceNo { get; set; }

        public List<TempAnswerChoiceViewModel> Choices { get; set; }

        public List<TempAnswerDescriptiveViewModel> Descriptive { get; set; }

        public List<TempAnswerMatchPairViewModel> Matches { get; set; }


    }

    public class TempTestQuestionViewModel
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public int PoolID { get; set; }
        public string PoolName { get; set; }
        public int QuestionID { get; set; }
        public int QuestionType { get; set; }
        public string QuestionText { get; set; }
        public int PaperID { get; set; }
        public int TestSetNo { get; set; }
        public int SequenceNo { get; set; }

        public List<TempAnswerChoiceViewModel> Choices { get; set; }

        public List<TempAnswerDescriptiveViewModel> Descriptive { get; set; }

        public List<TempAnswerMatchPairViewModel> Matches { get; set; }


    }
    public class TempAnswerChoiceViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string Text { get; set; }
        public bool IsAnswer { get; set; }
        public int PointsPerChoice { get; set; }
        public int DisplayType { get; set; }
        public int PaperID { get; set; }
        public int TempTestQuestionID { get; set; }

     
    }

    public class TempAnswerDescriptiveViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public string Keywords { get; set; }
        public int PositivePoints { get; set; }
        public int NegativePoints { get; set; }
        public int IsAnswer { get; set; }
        public int PaperID { get; set; }
        public int TempTestQuestionID { get; set; }
    }

    public class TempAnswerMatchPairViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string ChoiceA { get; set; }
        public string ChoiceB { get; set; }
        public string DisplayB { get; set; }
        public int PaperID { get; set; }
        public int TempTestQuestionID { get; set; }
    }

}