using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Library.Data.DataConstants;

namespace Library.Data.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(BookTitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(BookAuthorMaxLength)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(BookDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength((int)BookRatingMaxValue)]
        public decimal Rating { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public IList<IdentityUserBook> UsersBooks { get; set; } = new List<IdentityUserBook>();
    }
}
