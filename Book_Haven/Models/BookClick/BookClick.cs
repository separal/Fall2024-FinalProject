using System;

namespace Fall2024_Assignment3_separal.Models
{
    public class BookClick
    {
        public int ID { get; set; } // Primary Key
        public int BookID { get; set; } // Foreign Key to the Book table
        public DateTime ClickTime { get; set; } // Timestamp for the click

        // Navigation property
        public Book Book { get; set; }
    }
}

