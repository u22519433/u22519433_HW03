using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u22519433_HW03.ViewModels
{
    public class BorrowingHistoryReport
    {
        public string StudentName { get; set; }
        public string BookName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int DaysKept { get; set; }
        public bool IsOverdue { get; set; }

    }
}