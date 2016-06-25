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
    public class TopicViewModelValidators : AbstractValidator<TopicViewModel>
    {
        public TopicViewModelValidators()
        {
            RuleFor(r => r.Code).NotEmpty()
                .WithMessage("Invalid Topic Code");
            RuleFor(r => r.Name).NotEmpty()
                .WithMessage("Invalid Topic Name");
            RuleFor(r => r.Objective).NotEmpty()
                .WithMessage("Invalid Topic Objective");
        }
    }
}
