using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.ViewModels.Shop
{
    public class ShopVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        
    }
}
