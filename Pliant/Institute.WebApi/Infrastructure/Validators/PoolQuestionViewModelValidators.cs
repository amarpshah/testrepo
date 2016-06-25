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
    public class PoolQuestionViewModelValidators : AbstractValidator<PoolQuestionViewModel>
    {
        public PoolQuestionViewModelValidators()
        {
            RuleFor(r => r.PoolID).NotEmpty()
                .WithMessage("Invalid Pool");
        }
    }
}
