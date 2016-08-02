using FluentValidation;
using Institute.Entities;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.WebApi.Infrastructure.Validators
{
    public class SubjectViewModelValidators : AbstractValidator<SubjectViewModel>
    {
        public SubjectViewModelValidators()
        {
            RuleFor(r => r.Code).NotEmpty()
               .WithMessage("Invalid Code");
            RuleFor(r => r.Subject).NotEmpty()
                .WithMessage("Invalid Subject Name");
        }
    }
}
