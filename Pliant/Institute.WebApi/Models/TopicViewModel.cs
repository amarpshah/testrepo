using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class TopicViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public int MappingID { get; set; }
        public string Standard { get; set; }
        public int StandardId { get; set; }
        public string Subject { get; set; }
        public int SubjectId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Objective { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new TopicViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}