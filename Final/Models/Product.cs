using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [StringLength(1000)]
        public string Image { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public IEnumerable<ProductTag> ProductTags { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Basket> Baskets { get; set; }
        public List<Review> Reviews { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        [NotMapped]
        public List<int> Counts { get; set; } = new List<int>();
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
