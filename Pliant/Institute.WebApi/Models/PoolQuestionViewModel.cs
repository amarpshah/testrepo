using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class PoolQuestionViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int PoolID { get; set; }
        public int QuestionID { get; set; }
        public bool IsMandatory { get; set; }

        public PoolViewModel Pool { get; set; }
        public QuestionViewModel Questions { get; set; }
        
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var validator = new PoolQuestionViewModelValidators();
        //    var result = validator.Validate(this);
        //    return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        //}
    }
}