using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class MappingViewModel //: IValidatableObject
    {
        public int ID { get; set; }
        public int StandardID { get; set; }        
        public int SubjectID { get; set; }
        public string Standard { get; set; }
        public string Division { get; set; }
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public int TopicCount { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var validator = new SubjectViewModelValidators();
        //    var result = validator.Validate(this);
        //    return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        //}
    }
}