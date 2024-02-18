using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.DataConstants.CategoryConstants;

namespace SeminarHub.Data.Models
{
    public class Category
    {
        [Key]
        [Comment("Category identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Category name")]
        public string Name { get; set; } = string.Empty;

        [Comment("Collection of seminars")]
        public IEnumerable<Seminar> Seminars { get; set; } = new List<Seminar>();
    }
}
