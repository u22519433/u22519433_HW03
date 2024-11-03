using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using u22519433_HW03.DAL;
using u22519433_HW03.Models;
using u22519433_HW03.ViewModels;
using Type = u22519433_HW03.Models.Type;

namespace LibraryManagement.Controllers
{
    public class MaintainController : Controller
    {
        private LibraryContext db = new LibraryContext();

        // GET: Maintain
        public async Task<ActionResult> Index()
        {
            var viewModel = new MaintainViewModel
            {
                Types = await db.Types.ToListAsync(),
                Authors = await db.Authors.ToListAsync(),
                Borrows = await db.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Student)
                    .ToListAsync()
            };
            ViewBag.StudentId = new SelectList(await db.Students.ToListAsync(), "StudentId", "Name");
            ViewBag.BookId = new SelectList(await db.Books.ToListAsync(), "BookId", "Name");

            return View(viewModel);
        }

        // POST: Create Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateType([Bind(Include = "Name")] Type type)
        {
            if (ModelState.IsValid)
            {
                db.Types.Add(type);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView("_CreateTypeModal", type);
        }

        // POST: Create Author
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAuthor([Bind(Include = "Name,Surname")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Authors.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView("_CreateAuthorModal", author);
        }

        // POST: Create Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBorrow([Bind(Include = "StudentId,BookId,TakenDate")] Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.Borrows.Add(borrow);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(await db.Books.ToListAsync(), "BookId", "Name");
            ViewBag.StudentId = new SelectList(await db.Students.ToListAsync(), "StudentId", "Name");
            return PartialView("_CreateBorrowModal", borrow);
        }

        // POST: Update Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateType([Bind(Include = "TypeId,Name")] Type type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(type).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView("_UpdateTypeModal", type);
        }

        // POST: Update Author
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAuthor([Bind(Include = "AuthorId,Name,Surname")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView("_UpdateAuthorModal", author);
        }

        // POST: Update Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateBorrow([Bind(Include = "BorrowId,StudentId,BookId,TakenDate,BroughtDate")] Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrow).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(await db.Books.ToListAsync(), "BookId", "Name");
            ViewBag.StudentId = new SelectList(await db.Students.ToListAsync(), "StudentId", "Name");
            return PartialView("_UpdateBorrowModal", borrow);
        }

        // POST: Delete Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteType(int id)
        {
            try
            {
                var type = await db.Types.FindAsync(id);
                if(type == null){
                    return Json(new { success = false, message = "Type not found" });
                }

                db.Types.Remove(type);
                await db.SaveChangesAsync();
                return Json(new { success = true }); 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Delete Author
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await db.Authors.FindAsync(id);
                if (author == null)
                {
                    return Json(new { success = false, message = "Author not found" }); 
                }

                db.Authors.Remove(author);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new {success = false,  message = ex.Message});
            }

        }

        // POST: Delete Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBorrow(int id)
        {
            Borrow borrow = await db.Borrows.FindAsync(id);
            db.Borrows.Remove(borrow);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

