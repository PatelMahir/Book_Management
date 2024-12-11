using System.ComponentModel.DataAnnotations;
namespace Book_Management.Domain.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
