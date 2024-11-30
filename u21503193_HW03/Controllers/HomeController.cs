using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u21503193_HW03.Models;
using PagedList;
using System.Web.UI;
using PagedList.EntityFramework;

namespace u21503193_HW03.Controllers
{
    public class HomeController : Controller
    {
        LibraryEntities db = new LibraryEntities();

        public async Task<ActionResult> Index(int? studentPage, int? bookPage)
        {
            int pageSize = 10;

            int studentPageNumber = studentPage ?? 1;
            int bookPageNumber = bookPage ?? 1;

            var students = await db.students.OrderBy(s => s.studentId).ToPagedListAsync(studentPageNumber, pageSize);
            var books = await db.books.OrderBy(b => b.bookId).ToPagedListAsync(bookPageNumber, pageSize);
            var borrows = await db.borrows.ToListAsync();

            foreach (var book in books)
            {
                var borrowRecord = borrows.FirstOrDefault(b => b.bookId == book.bookId && b.broughtDate == null);
                if (borrowRecord != null)
                {
                    book.BookStatus = Status.Out;
                }
                else
                {
                    book.BookStatus = Status.Available;
                }
            }

            var viewModel = new CombinedHomeVM
            {
                Students = students,
                Books = books
            };

            return View(viewModel);
        }

        // GET: students
        public async Task<ActionResult> studentsIndex()
        {
            return View(await db.students.ToListAsync());
        }

        // GET: students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> studentsCreate([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students students)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(students);
                await db.SaveChangesAsync();
                return RedirectToAction("studentsIndex");
            }

            return View(students);
        }

        // GET: books
        public async Task<ActionResult> booksIndex()
        {
            var books = db.books.Include(b => b.authors).Include(b => b.types);
            return View(await books.ToListAsync());
        }

        // GET: books/Create
        public ActionResult booksCreate()
        {
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name");
            ViewBag.typeId = new SelectList(db.types, "typeId", "name");
            return View();
        }

        // POST: books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> booksCreate([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books books)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(books);
                await db.SaveChangesAsync();
                return RedirectToAction("booksIndex");
            }

            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}