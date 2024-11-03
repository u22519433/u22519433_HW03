using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u22519433_HW03.Models;

namespace u22519433_HW03.ViewModels
{
    public class HomeViewModel
    {
       
            public List<Student> Students { get; set; }
            public List<Book> Books { get; set; }
            public List<Borrow> Borrows { get; set; }


    }
}