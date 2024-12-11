using Book_Management.DBContext;
using Book_Management.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Book_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookController> _logger;

        public BookController(ApplicationDbContext context, ILogger<BookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Book
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.Author)
                    .ToListAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                return StatusCode(500, "An error occurred while retrieving books");
            }
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID {id}");
                return StatusCode(500, "An error occurred while retrieving the book");
            }
        }

        // POST: api/Book
        [HttpPost]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validate Author
                var author = await _context.Authors.FindAsync(book.AuthorId);
                if (author == null)
                {
                    return BadRequest($"Author with ID {book.AuthorId} does not exist");
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // Reload the book with author to return full object
                var createdBook = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.Id == book.Id);

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, createdBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "An error occurred while creating the book");
            }
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest("Mismatched book ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verify book exists
                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                // Verify author exists
                var author = await _context.Authors.FindAsync(book.AuthorId);
                if (author == null)
                {
                    return BadRequest($"Author with ID {book.AuthorId} does not exist");
                }

                // Update book properties
                existingBook.Title = book.Title;
                existingBook.ISBN = book.ISBN;
                existingBook.PublicationYear = book.PublicationYear;
                existingBook.AuthorId = book.AuthorId;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {id}");
                return StatusCode(500, "An error occurred while updating the book");
            }
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID {id}");
                return StatusCode(500, "An error occurred while deleting the book");
            }
        }

        // Search Books
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(
            [FromQuery] string? title,
            [FromQuery] string? isbn,
            [FromQuery] int? publicationYear,
            [FromQuery] int? authorId)
        {
            try
            {
                var query = _context.Books
                    .Include(b => b.Author)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(b => b.Title.Contains(title));

                if (!string.IsNullOrWhiteSpace(isbn))
                    query = query.Where(b => b.ISBN == isbn);

                if (publicationYear.HasValue)
                    query = query.Where(b => b.PublicationYear == publicationYear);

                if (authorId.HasValue)
                    query = query.Where(b => b.AuthorId == authorId);

                var results = await query.ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books");
                return StatusCode(500, "An error occurred while searching books");
            }
        }
    }
}
