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
    public class PoolViewModelValidators : AbstractValidator<PoolViewModel>
    {
        public PoolViewModelValidators()
        {
            RuleFor(r => r.Name).NotEmpty()
                .WithMessage("Invalid Pool");
        }
    }
}
