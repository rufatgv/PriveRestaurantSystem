﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class BlogTag
    {
        public int Id { get; set; }
        public Nullable<int> BlogId { get; set; }
        public Nullable<int> TagId { get; set; }
        public Blog Blog { get; set; }
        public Tag Tag { get; set; }
    }
}
