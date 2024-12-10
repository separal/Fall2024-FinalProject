using System.Collections.Generic;
using System.Reflection.Emit;
using Fall2024_Assignment3_separal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fall2024_Assignment3_separal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } // DbSet for Books
        public DbSet<BookClick> BookClicks { get; set; } // DbSet for BookClicks

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map BookClick table and relationships
            modelBuilder.Entity<BookClick>()
                .ToTable("BookClicks") // Explicitly map the table
                .HasKey(bc => bc.ID); // Ensure EF Core knows the primary key

            modelBuilder.Entity<BookClick>()
                .HasOne(bc => bc.Book) // Navigation property to Book
                .WithMany(b => b.BookClicks) // Book has many BookClicks
                .HasForeignKey(bc => bc.BookID); // Foreign key to Book
        }
    }
}
