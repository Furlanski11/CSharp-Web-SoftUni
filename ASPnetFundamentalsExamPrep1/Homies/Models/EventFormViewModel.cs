using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Homies.Data.DataConstants;
using static Homies.Data.ErrorMessages;

namespace Homies.Models
{
    public class EventFormViewModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(NameMaxLength, 
            MinimumLength = NameMinLength, 
            ErrorMessage = StringLengthErrorMessage)]

        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DescriptionMaxLength,
            MinimumLength = DescriptionMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Description { get; set; } = string.Empty;


        [Required(ErrorMessage = RequireErrorMessage)]
        public string Start { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string End { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public int TypeId { get; set; }


        public IEnumerable<TypeViewModel> Types { get; set; } = new List<TypeViewModel>();
    }
}
