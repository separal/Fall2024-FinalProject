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

        public DbSet<Book> Books { get; set; } // Add the DbSet for Books

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            // Add additional configurations for the Book entity if needed
            modelBuilder.Entity<Book>()
                .HasKey(b => b.ID); // Ensure Book ID is the primary key
        }
    }
}
