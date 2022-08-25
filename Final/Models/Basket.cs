using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Basket:BaseEntity
    {
        public Nullable<int> ProductId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }


    }
}
