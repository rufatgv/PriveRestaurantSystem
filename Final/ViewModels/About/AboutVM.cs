using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.ViewModels.About
{
    public class AboutVM
    {
        public IEnumerable<AboutIntro> AboutIntros { get; set; }
        public IEnumerable<Mission> Missions { get; set; }
        public IEnumerable<Vision> Visions { get; set; }
    }
}
