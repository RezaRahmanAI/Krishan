using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateDepotViewModelValidator : AbstractValidator<CreateDepotViewModel>
    {
        public CreateDepotViewModelValidator()
        {
            RuleFor(Depot => Depot.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 250)
                .WithMessage("Code must be between 1 and 250 in length.");

            RuleFor(Depot => Depot.Name)
                .NotEmpty()
                .WithMessage("Depot is required.")
                .Length(1, 250)
                .WithMessage("Depot must be between 1 and 250 in length.");
        }
    }
}