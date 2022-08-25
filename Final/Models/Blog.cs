using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Blog:BaseEntity
    {
        [StringLength(1000)]
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public IEnumerable<BlogTag> BlogTags { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public Category Category { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
