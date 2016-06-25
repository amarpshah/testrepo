using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class QuestionViewModel : IValidatableObject
    {
        public bool IsChecked { get; set; }

        public int ID { get; set; }
        public int TopicID { get; set; }

        public int StandardID { get; set; }
        public int SubjectID { get; set; }
        public int MappingID { get; set; }
        
        public string Standard { get; set; }
        public string Subject { get; set; }
        public string TopicName { get; set; }

        public string Code { get; set; }
        public string Text { get; set; }
        public string Objective { get; set; }
        public string Hint { get; set; }

        public int Type { get; set; }
        public string sType { get; set; }
        public string sTypeShort { get; set; }
        public int Points { get; set; }
        public int Status { get; set; }
        public string sStatus { get; set; }
        public int DifficultyLevel { get; set; }
        public string sDifficultyLevel { get; set; }
        public int Randomize { get; set; }

        public bool IsMappedToPool { get; set; }
        public bool IsActive { get; set; }

        
        public int IsLock { get; set; }
        public int LockedBy { get; set; }
        public string UserName { get; set; }
        public DateTime OnCreated { get; set; }
        public DateTime OnLocked { get; set; }
        public DateTime OnUpdated { get; set; }

        public List<AnswerChoiceViewModel> Choices { get; set; }

        public List<AnswerDescriptiveViewModel> Descriptive { get; set; }

        public List<AnswerMatchPairViewModel> Matches { get; set; }

        public List<PoolQuestionViewModel> PoolQuestionMap { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new QuestionViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class AnswerChoiceViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string Text { get; set; }
        public bool IsAnswer { get; set; }
        public int PointsPerChoice { get; set; }
        public int DisplayType { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    return null;
        //}
    }

    public class AnswerDescriptiveViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public string Keywords { get; set; }
        public int PositivePoints { get; set; }
        public int NegativePoints { get; set; }
        public int IsAnswer { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class AnswerMatchPairViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public string ChoiceA { get; set; }
        public string ChoiceB { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    throw new NotImplementedException();
        //}
    }

}