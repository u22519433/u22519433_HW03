using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace u22519433_HW03.Models
{
    public class Borrow
    {
        [Key]
        public int BorrowId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Borrowed Date")]
        [DataType(DataType.DateTime)]
        public DateTime TakenDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.DateTime)]
        public DateTime? BroughtDate { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Book Book { get; set; }

    }
}