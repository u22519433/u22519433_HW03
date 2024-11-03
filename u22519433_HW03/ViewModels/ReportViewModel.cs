using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u22519433_HW03.Models;

namespace u22519433_HW03.ViewModels
{
    public class ReportViewModel
    {
        public List<BorrowingHistoryReport> BorrowingHistory { get; set; }
        public List<SavedReport> SavedReports { get; set; }
        public string SelectedReportType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }


}