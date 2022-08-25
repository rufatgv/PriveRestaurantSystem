using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.ViewModels.Blog
{
    public class BlogVM
    {
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        public Final.Models.Blog Blog { get; set; }
        public List<Final.Models.Blog> Blogs { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
