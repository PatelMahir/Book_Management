using Book_Management.DBContext;
using Book_Management.Domain.Models;
namespace Book_Management.GraphQL
{
    public class Mutation
    {
        private readonly ApplicationDbContext _dbContext;
        public Mutation(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Author> AddAuthor(
            string name,
            string email,
            ApplicationDbContext context)
        {
            var author = new Author
            {
                Name = name,
                Email = email
            };

            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            return author;
        }

        public async Task<Book> AddBook(
            string title,
            string isbn,
            int publicationYear,
            int authorId,
            ApplicationDbContext context)
        {
            var book = new Book
            {
                Title = title,
                ISBN = isbn,
                PublicationYear = publicationYear,
                AuthorId = authorId
            };

            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return book;
        }
    }
}
