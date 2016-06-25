using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Institute.WebApi.Models
{
//    [Bind(Exclude = "UniqueKey")]
    public class StudentViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Standard { get; set; }
        public int StandardId { get; set; }
        public string Image { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new StudentViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}