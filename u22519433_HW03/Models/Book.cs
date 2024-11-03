using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace u22519433_HW03.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Book Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Page Count")]
        public int PageCount { get; set; }

        [Required]
        public int Point { get; set; }

        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        [Required]
        [ForeignKey("Type")]
        public int TypeId { get; set; }

        // Navigation properties
        public virtual Author Author { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<Borrow> Borrows { get; set; }

    }
}