using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class HomeIntro:BaseEntity
    {
        public string Intro { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
