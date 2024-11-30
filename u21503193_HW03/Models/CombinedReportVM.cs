using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21503193_HW03.Models
{
    public class CombinedReportVM
    {
        public IPagedList<authors> Authors { get; set; }
        public IPagedList<types> Types { get; set; }
        public IPagedList<borrows> Borrows { get; set; }
    }
}