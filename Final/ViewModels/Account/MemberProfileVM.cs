using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.ViewModels.Account
{
    public class MemberProfileVM
    {
        public MemberUpdateVM Member { get; set; }
        public List<Final.Models.Order> Orders { get; set; }
    }
}
