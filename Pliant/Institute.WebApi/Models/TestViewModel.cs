using Institute.WebApi.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Institute.Entities;

namespace Institute.WebApi.Models
{
    public class TestViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public int Status { get; set; }
        public string sStatus { get; set; }
        public int NegativeMarks { get; set; }
        public int PointPerQuestion { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public int ScoringMode { get; set; }
        public int DifficultyLevel { get; set; }
        public string sDifficultyLevel { get; set; }
        public int PoolSequence { get; set; }
        public int PoolCount { get; set; }
        public int ShowHint { get; set; }
        public int Lock { get; set; }
        public int LockedBy { get; set; }
        public string UserName { get; set; }
        public DateTime OnLocked { get; set; }
        public DateTime OnCreated { get; set; }
        public DateTime OnUpdated { get; set; }

        //public List<PoolViewModel> Pools { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new TestViewModelValidators();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}