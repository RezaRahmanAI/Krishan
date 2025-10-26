using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CCBankViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Code { get; set; }

        [Display(Name = "Bank Name")]
        public string Name { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CCBankViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}