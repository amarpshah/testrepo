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
    public class RoleViewModelValidators : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidators()
        {
            RuleFor(r => r.Name).NotEmpty()
                .WithMessage("Invalid name");
        }
    }
}