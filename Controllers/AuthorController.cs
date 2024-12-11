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
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(ApplicationDbContext context, ILogger<AuthorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Author
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Author>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            try
            {
                var authors = await _context.Authors
                    .Include(a => a.Books)
                    .ToListAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving authors");
                return StatusCode(500, "An error occurred while retrieving authors");
            }
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Author), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound($"Author with ID {id} not found");
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author with ID {id}");
                return StatusCode(500, "An error occurred while retrieving the author");
            }
        }

        // POST: api/Author
        [HttpPost]
        [ProducesResponseType(typeof(Author), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Author>> CreateAuthor([FromBody] Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if author with same email already exists
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Email == author.Email);

                if (existingAuthor != null)
                {
                    return BadRequest($"An author with email {author.Email} already exists");
                }

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating author");
                return StatusCode(500, "An error occurred while creating the author");
            }
        }

        // PUT: api/Author/5
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author author)
        {
            if (id != author.Id)
            {
                return BadRequest("Mismatched author ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingAuthor = await _context.Authors.FindAsync(id);
                if (existingAuthor == null)
                {
                    return NotFound($"Author with ID {id} not found");
                }

                // Check if email is being changed to an existing email
                var emailConflict = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Email == author.Email && a.Id != id);

                if (emailConflict != null)
                {
                    return BadRequest($"Email {author.Email} is already in use by another author");
                }

                // Update author properties
                existingAuthor.Name = author.Name;
                existingAuthor.Email = author.Email;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating author with ID {id}");
                return StatusCode(500, "An error occurred while updating the author");
            }
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound($"Author with ID {id} not found");
                }

                // Prevent deletion if author has books
                if (author.Books != null && author.Books.Any())
                {
                    return Conflict($"Cannot delete author with ID {id} as they have associated books");
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting author with ID {id}");
                return StatusCode(500, "An error occurred while deleting the author");
            }
        }

        // Search Authors
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Author>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Author>>> SearchAuthors(
            [FromQuery] string? name,
            [FromQuery] string? email)
        {
            try
            {
                var query = _context.Authors
                    .Include(a => a.Books)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(a => a.Name.Contains(name));

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(a => a.Email.Contains(email));

                var results = await query.ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching authors");
                return StatusCode(500, "An error occurred while searching authors");
            }
        }
    }
}
