using Library.Data.Models;
using System.ComponentModel.DataAnnotations;
using static Library.Data.DataConstants;

namespace Library.Models
{
    public class AllBooksModel
    {
        public int Id { get; set; }
        
        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(BookTitleMaxLength,
            MinimumLength = BookTitleMinLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(BookAuthorMaxLength,
            MinimumLength = BookAuthorMinLength)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength((int)BookRatingMaxValue)]
        public decimal Rating { get; set; }


        [Required]
        public string Category { get; set; } = null!;
    }
}
