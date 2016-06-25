using FluentValidation;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Infrastructure.Validators
{
    public class StudentViewModelValidators : AbstractValidator<StudentViewModel>
    {
        public StudentViewModelValidators()
        {
            RuleFor(customer => customer.Firstname).NotEmpty()
                .Length(1, 100).WithMessage("First Name must be between 1 - 100 characters");

            RuleFor(customer => customer.Lastname).NotEmpty()
                .Length(1, 100).WithMessage("Last Name must be between 1 - 100 characters");

            RuleFor(customer => customer.DateOfBirth).NotNull()
                .LessThan(DateTime.Now.AddYears(-1))
                .WithMessage("Student must be at least 1 years old.");
        }
    }
}