using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.DataConstants.SeminarConstants;
namespace SeminarHub.Models
{
    public class EditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(TopicMaxLength, MinimumLength = TopicMinLength, ErrorMessage = StringLengthErrorMessage)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength, ErrorMessage = StringLengthErrorMessage)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength, ErrorMessage = StringLengthErrorMessage)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = RequireErrorMessage)]
        [DisplayFormat(DataFormatString = DateFormat)]
        public string DateAndTime { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [Range(DurationMinLength, DurationMaxLength)]
        public int Duration { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
