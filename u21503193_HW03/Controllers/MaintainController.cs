using PagedList.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u21503193_HW03.Models;

namespace u21503193_HW03.Controllers
{
    public class MaintainController : Controller
    {
        LibraryEntities db = new LibraryEntities();

        public async Task<ActionResult> Index(int? authorPage, int? typePage, int? borrowPage)
        {
            int pageSize = 10;

            int authorPageNumber = authorPage ?? 1;
            int typePageNumber = typePage ?? 1;
            int borrowPageNumber = borrowPage ?? 1;

            var authors = await db.authors.OrderBy(a => a.authorId).ToPagedListAsync(authorPageNumber, pageSize);
            var types = await db.types.OrderBy(t => t.typeId).ToPagedListAsync(typePageNumber, pageSize);
            var borrows = await db.borrows.OrderBy(b => b.borrowId).ToPagedListAsync(borrowPageNumber, pageSize);

            var viewModel = new CombinedReportVM
            {
                Authors = authors,
                Types = types,
                Borrows = borrows
            };

            return View(viewModel);
        }
    }
}