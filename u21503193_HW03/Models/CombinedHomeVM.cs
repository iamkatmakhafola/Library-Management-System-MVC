﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21503193_HW03.Models
{
    public class CombinedHomeVM
    {
        public IPagedList<books> Books {  get; set; }
        public IPagedList<students> Students { get; set; }
    }
}