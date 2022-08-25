using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Table:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; }
        public string MainEmail { get; set; } 
        public string Phone { get; set; }

        [Required]
        public int Person { get; set; }
        public DateTime Date { get; set; }

    }
}
