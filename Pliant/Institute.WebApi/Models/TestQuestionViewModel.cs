using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class TestQuestionViewModel
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public int PoolID { get; set; }
        public string PoolName { get; set; }
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int PaperID { get; set; }
        public int TestSetNo { get; set; }
        public int SequenceNo { get; set; }

        public List<FinalAnswerChoiceViewModel> Choices { get; set; }

        public List<FinalAnswerDescriptiveViewModel> Descriptive { get; set; }

        public List<FinalAnswerMatchPairViewModel> Matches { get; set; }


    }

    public class FinalAnswerChoiceViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string Text { get; set; }
        public bool IsAnswer { get; set; }
        public int PointsPerChoice { get; set; }
        public int DisplayType { get; set; }
        public int PaperID { get; set; }
        public int TestQuestionID { get; set; }


    }

    public class FinalAnswerDescriptiveViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public string Keywords { get; set; }
        public int PositivePoints { get; set; }
        public int NegativePoints { get; set; }
        public int IsAnswer { get; set; }
        public int PaperID { get; set; }
        public int TestQuestionID { get; set; }
    }

    public class FinalAnswerMatchPairViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string ChoiceA { get; set; }
        public string ChoiceB { get; set; }
        public string DisplayB { get; set; }
        public int PaperID { get; set; }
        public int TestQuestionID { get; set; }
    }

}