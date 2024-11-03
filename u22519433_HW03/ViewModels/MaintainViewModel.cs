using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u22519433_HW03.Models;
using Type = u22519433_HW03.Models.Type;

namespace u22519433_HW03.ViewModels
{
    public class MaintainViewModel
    {
        public List<Type> Types { get; set; }
        public List<Author> Authors { get; set; }
        public List<Borrow> Borrows { get; set; }

    }
}