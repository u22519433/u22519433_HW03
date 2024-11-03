using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using u22519433_HW03.DAL;
using u22519433_HW03.Models;
using u22519433_HW03.ViewModels;
using System.Web.Hosting; 

    


namespace LibraryManagement.Controllers
    {
        public class ReportController : Controller
        {
            private LibraryContext db = new LibraryContext();
            private string uploadDirectory = "~/App_Data/Reports/";

            public ReportController()
            {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath(uploadDirectory);
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }
            }

            // GET: Report
            public async Task<ActionResult> Index()
            {
                var viewModel = new ReportViewModel
                {
                    SavedReports = await db.SavedReports.OrderByDescending(r => r.SavedDate).ToListAsync()
                };
            
            return View(viewModel);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateReport(string reportType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                switch (reportType)
                {
                    case "BorrowingHistory":
                        var borrowHistory = await GenerateBorrowingHistoryReport(startDate, endDate);
                        return Json(new { success = true, items = borrowHistory });

                    case "OverdueBooks":
                        var overdueBooks = await db.Borrows
                            .Include(b => b.Student)
                            .Include(b => b.Book)
                            .Where(b => b.BroughtDate == null &&
                                   DbFunctions.DiffDays(b.TakenDate, DateTime.Now) > 14)
                            .Select(b => new
                            {
                                studentName = b.Student.Name + " " + b.Student.Surname,
                                bookName = b.Book.Name,
                                borrowDate = b.TakenDate,
                                daysOverdue = DbFunctions.DiffDays(b.TakenDate, DateTime.Now).Value - 14
                            })
                            .ToListAsync();
                        return Json(new { success = true, items = overdueBooks });

                    case "PopularBooks":
                        var totalBorrows = await db.Borrows.CountAsync();
                        var popularBooks = await db.Books
                            .Select(b => new
                            {
                                bookName = b.Name,
                                borrowCount = b.Borrows.Count(),
                                isAvailable = !b.Borrows.Any(br => br.BroughtDate == null),
                                popularityScore = totalBorrows > 0 ?
                                    (b.Borrows.Count() * 100) / totalBorrows : 0
                            })
                            .OrderByDescending(b => b.borrowCount)
                            .Take(10)
                            .ToListAsync();
                        return Json(new { success = true, items = popularBooks });

                    case "UnreturnedBooks":
                        var unreturnedBooks = await db.Borrows
                            .Include(b => b.Student)
                            .Include(b => b.Book)
                            .Where(b => b.BroughtDate == null)
                            .Select(b => new
                            {
                                bookName = b.Book.Name,
                                studentName = b.Student.Name + " " + b.Student.Surname,
                                borrowDate = b.TakenDate,
                                daysOutstanding = DbFunctions.DiffDays(b.TakenDate, DateTime.Now)
                            })
                            .OrderByDescending(b => b.daysOutstanding)
                            .ToListAsync();
                        return Json(new { success = true, items = unreturnedBooks });

                    default:
                        return Json(new { success = false, message = "Invalid report type" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<List<BorrowingHistoryReport>> GenerateBorrowingHistoryReport(DateTime? startDate, DateTime? endDate)
        {
            var query = db.Borrows
                .Include(b => b.Student)
                .Include(b => b.Book)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(b => b.TakenDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(b => b.TakenDate <= endDate.Value);

            return await query
                .Select(b => new BorrowingHistoryReport
                {
                    StudentName = b.Student.Name + " " + b.Student.Surname,
                    BookName = b.Book.Name,
                    BorrowDate = b.TakenDate,
                    ReturnDate = b.BroughtDate,
                    DaysKept = b.BroughtDate.HasValue ?
                        DbFunctions.DiffDays(b.TakenDate, b.BroughtDate.Value).Value :
                        DbFunctions.DiffDays(b.TakenDate, DateTime.Now).Value,
                    IsOverdue = !b.BroughtDate.HasValue &&
                        DbFunctions.DiffDays(b.TakenDate, DateTime.Now) > 14
                })
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }




        [HttpPost]
            public async Task<ActionResult> SaveReport(string reportData, string fileName, string fileType)
            {
                try
                {
                    string filePath = Path.Combine(Server.MapPath(uploadDirectory),
                        $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}.{fileType}");

                    // Save the report based on file type
                    switch (fileType.ToLower())
                    {
                        case "pdf":
                            await SaveAsPdf(reportData, filePath);
                            break;
                        case "txt":
                        await Task.Run(() => System.IO.File.WriteAllText(filePath, reportData));
                            break;
                    }

                    // Save report metadata to database
                    var savedReport = new SavedReport
                    {
                        FileName = fileName,
                        FileType = fileType,
                        SavedDate = DateTime.Now,
                        FilePath = filePath,
                        Description = ""  // Will be updated later via UpdateDescription
                    };

                    db.SavedReports.Add(savedReport);
                    await db.SaveChangesAsync();

                    return Json(new { success = true, reportId = savedReport.ReportId });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            private async Task SaveAsPdf(string reportData, string filePath)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();
                    document.Add(new Paragraph(reportData));
                    document.Close();
                }
            }

            [HttpPost]
            public async Task<ActionResult> UpdateDescription(int reportId, string description)
            {
                var report = await db.SavedReports.FindAsync(reportId);
                if (report != null)
                {
                    report.Description = description;
                    await db.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }

            public async Task<ActionResult> DownloadReport(int id)
            {
                var report = await db.SavedReports.FindAsync(id);
                if (report == null)
                    return HttpNotFound();

                string filePath = report.FilePath;
                string fileName = Path.GetFileName(filePath);
                string contentType = GetContentType(report.FileType);

                return File(filePath, contentType, fileName);
            }

            [HttpPost]
            public async Task<ActionResult> DeleteReport(int id)
            {
                var report = await db.SavedReports.FindAsync(id);
                if (report == null)
                    return HttpNotFound();

                // Delete physical file
                if (System.IO.File.Exists(report.FilePath))
                    System.IO.File.Delete(report.FilePath);

                // Delete database record
                db.SavedReports.Remove(report);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }

            private string GetContentType(string fileType)
            {
                switch (fileType.ToLower())
                {
                    case "pdf":
                        return "application/pdf";
                    case "txt":
                        return "text/plain";
                    default:
                        return "application/octet-stream";
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }


