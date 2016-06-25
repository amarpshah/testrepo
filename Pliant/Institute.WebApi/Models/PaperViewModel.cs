using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Institute.WebApi.Infrastructure.Validators;

namespace Institute.WebApi.Models
{
    public class PaperViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public int NoOfSets { get; set; }
        public bool IsFinalized { get; set; }
        public int CreatedBy { get; set; }
        public DateTime OnCreated { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new PaperViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}