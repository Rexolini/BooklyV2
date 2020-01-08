using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class RolaViewModel
    {
        [Required]
        public string Nazwa { get; set; }
    }
}