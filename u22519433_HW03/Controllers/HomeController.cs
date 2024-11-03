using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using u22519433_HW03.DAL;
using u22519433_HW03.Models;
using u22519433_HW03.ViewModels;

    namespace u22519433_HW03.Controllers
    {
        public class HomeController : Controller
        {
            private LibraryContext db = new LibraryContext();

            // GET: Home
            public async Task<ActionResult> Index()
            {
                var viewModel = new HomeViewModel
                {
                    Students = await db.Students.ToListAsync(),
                    Books = await db.Books
                        .Include(b => b.Author)
                        .Include(b => b.Type)
                        .ToListAsync(),
                    Borrows = await db.Borrows
                        .Include(b => b.Book)
                        .Where(b => b.BroughtDate == null)
                        .ToListAsync()
                };

            ViewBag.AuthorId = new SelectList(await db.Authors.ToListAsync(), "AuthorId", "Name");
            ViewBag.TypeId = new SelectList(await db.Types.ToListAsync(), "TypeId", "Name");

                return View(viewModel);
            }

            // POST: Create Student (Modal)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> CreateStudent([Bind(Include = "Name,Surname,Birthdate,Gender,Class,Point")] Student student)
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return PartialView("_CreateStudentModal", student);
            }

            // POST: Create Book (Modal)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> CreateBook([Bind(Include = "Name,PageCount,Point,AuthorId,TypeId")] Book book)
            {
                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ViewBag.AuthorId = new SelectList(await db.Authors.ToListAsync(), "AuthorId", "Name", book.AuthorId);
                ViewBag.TypeId = new SelectList(await db.Types.ToListAsync(), "TypeId", "Name", book.TypeId);
                return PartialView("_CreateBookModal", book);
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
