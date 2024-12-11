using Book_Management.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Book_Management.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            // Seed data
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "J.K. Rowling", Email = "jk.rowling@example.com" },
                new Author { Id = 2, Name = "Stephen King", Email = "stephen.king@example.com" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Harry Potter", ISBN = "1234567890", PublicationYear = 1997, AuthorId = 1 },
                new Book { Id = 2, Title = "The Shining", ISBN = "0987654321", PublicationYear = 1977, AuthorId = 2 }
            );
        }
    }
}
