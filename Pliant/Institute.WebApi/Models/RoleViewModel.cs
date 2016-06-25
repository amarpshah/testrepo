using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Models
{
    public class RoleViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new RoleViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}