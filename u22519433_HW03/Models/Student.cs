using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace u22519433_HW03.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Class")]
        public string Class { get; set; }

        [Required]
        public int Point { get; set; }

        // Navigation property
        public virtual ICollection<Borrow> Borrows { get; set; }

    }
}