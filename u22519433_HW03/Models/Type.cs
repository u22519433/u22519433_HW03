using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace u22519433_HW03.Models
{
    public class Type
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        [Display(Name = "Genre")]
        public string Name { get; set; }

        // Navigation property
        public virtual ICollection<Book> Books { get; set; }

    }
}