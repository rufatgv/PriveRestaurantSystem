using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.ViewModels.Home
{
    public class HomeVM
    {
        public IEnumerable<Team> Teams { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<HomeIntro> HomeIntros { get; set; }
        //public Final.Models.Table Tables { get; set; }
    }
}
