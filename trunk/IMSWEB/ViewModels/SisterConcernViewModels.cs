using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;
namespace IMSWEB
{
    public class CreateSisterConcernViewModel : IValidatableObject
    {
        public CreateSisterConcernViewModel()
        {
            SisterConcerns = new List<SisterConcern>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public decimal ServiceCharge { get; set; }
        [Display(Name = "SMS Contact")]
        public string SmsContactNo { get; set; }
        [Display(Name = "Parent Concern")]
        public int? ParentID { get; set; }

        public List<SisterConcern> SisterConcerns { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateSisterConcernViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}