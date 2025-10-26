using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CCBankViewModelValidator : AbstractValidator<CCBankViewModel>
    {
        public CCBankViewModelValidator()
        {
            RuleFor(company => company.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 150)
                .WithMessage("Code must be between 1 and 150 in length.");

            RuleFor(company => company.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 350)
                .WithMessage("Name must be between 1 and 350 in length.");
        }
    }
}