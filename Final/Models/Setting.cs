using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Setting:BaseEntity
    {
        public string Logo { get; set; }
        public string Email { get; set; }
        [StringLength(255), Required]
        public string Phone { get; set; }
        [StringLength(255), Required]
        
        public string Address { get; set; }
        public string WorkHours { get; set; }
        public string ContactUsTitle { get; set; }
        public string ContactUsDescription { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        [NotMapped]
        public IFormFile LogoImage { get; set; }
    }
}
