using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class PoolViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public int PoolID { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int IsMandatoryToPass { get; set; }
        public int NoOfQuestionsOutOf { get; set; }
        public int PassingScore { get; set; }
        public int NegativeMarks { get; set; }
        public int PoolTotalMarks { get; set; }
        public int RandomizeChoice { get; set; }
        public int RandomizeQuestion { get; set; }
        public int DifficultyLevel { get; set; }
        public int QuestionCount { get; set; }

        public int IsLock { get; set; }
        public int LockedBy { get; set; }
        public string UserName { get; set; }
        public DateTime OnCreated { get; set; }
        public DateTime OnLocked { get; set; }
        public DateTime OnUpdated { get; set; }

        public string sStatus { get; set; }
        public string sDifficultyLevel { get; set; }
        public TestViewModel Test { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new PoolViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
