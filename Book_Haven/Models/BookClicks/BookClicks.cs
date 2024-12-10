namespace Fall2024_Assignment3_separal.Models
{
    public class BookClicks
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public DateTime ClickDate { get; set; }

        // Navigation property
        public Book Book { get; set; }

        // Additional properties for reporting
        public string Title { get; set; }
        public int ClickCount { get; set; }
    }
}