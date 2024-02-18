using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeminarHub.Data.DataConstants.SeminarConstants;

namespace SeminarHub.Data.Models
{
    public class Seminar
    {
        [Key]
        [Comment("Seminar identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TopicMaxLength)]
        [Comment("Seminar topic")]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [MaxLength(LecturerMaxLength)]
        [Comment("Lecturer of the seminar")]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MaxLength(DetailsMaxLength)]
        [Comment("Details about the seminar")]
        public string Details { get; set; } = string.Empty;

        [Required]
        [Comment("Organizer idenfitifier")]
        public string OrganizerId { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        [Comment("Date and time of the seminar")]
        public DateTime DateAndTime { get; set; }

        [Required]
        [Range(DurationMinLength,DurationMaxLength)]
        [Comment("Duration of the seminar")]
        public int Duration { get; set; }

        [Required]
        [Comment("Category identifier")]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public IList<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}
