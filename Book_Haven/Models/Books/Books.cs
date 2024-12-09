using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_separal.Models
{
    public class Book
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
        public string Author { get; set; }

        [Range(1, 2100, ErrorMessage = "Year must be a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public decimal Rating { get; set; }

        [Required(ErrorMessage = "ImageLink is required.")]
        [Url(ErrorMessage = "ImageLink must be a valid URL.")]
        public string ImageLink { get; set; }
    }
}
