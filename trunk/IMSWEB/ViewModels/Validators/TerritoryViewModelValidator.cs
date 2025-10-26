using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateTerritoryViewModelValidator : AbstractValidator<CreateTerritoryViewModel>
    {
        public CreateTerritoryViewModelValidator()
        {
            RuleFor(Territory => Territory.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 250)
                .WithMessage("Code must be between 1 and 250 in length.");

            RuleFor(Territory => Territory.Name)
                .NotEmpty()
                .WithMessage("Territory is required.")
                .Length(1, 250)
                .WithMessage("Territory must be between 1 and 250 in length.");
        }
    }
}