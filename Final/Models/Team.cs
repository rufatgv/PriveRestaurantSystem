using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Team:BaseEntity
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
