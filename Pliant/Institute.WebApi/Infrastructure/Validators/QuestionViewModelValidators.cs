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
    public class QuestionViewModelValidators : AbstractValidator<QuestionViewModel>
    {
        public QuestionViewModelValidators()
        {
            RuleFor(r => r.Code).NotEmpty()
                .WithMessage("Invalid Code");
            RuleFor(r => r.Text).NotEmpty()
                .WithMessage("Invalid Text");
            RuleFor(r => r.Hint).NotEmpty()
                .WithMessage("Invalid Hint");
        }
    }
}
