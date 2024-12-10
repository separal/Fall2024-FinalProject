using System;

namespace Fall2024_Assignment3_separal.Models
{
    public class BookClick
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public DateTime ClickDate { get; set; }

        // Navigation property
        public Book Book { get; set; }
    }
}