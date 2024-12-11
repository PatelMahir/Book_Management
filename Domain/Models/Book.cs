using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Book_Management.Domain.Models
{
     public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }
    } 
}
