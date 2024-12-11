using Book_Management.DBContext;
using Book_Management.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Book_Management.GraphQL
{
    public class Query
    {
        private readonly ApplicationDbContext _dbContext;
        public Query(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IQueryable<Author>> GetAuthors(ApplicationDbContext context)
        {
            return _dbContext.Authors.Include(a => a.Books);
        }

        public async Task<IQueryable<Book>> GetBooks(ApplicationDbContext context)
        {
            return _dbContext.Books.Include(b => b.Author);
        }

        public async Task<Author?> GetAuthorById(int id, ApplicationDbContext context)
        {
            return await _dbContext.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
