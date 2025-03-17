using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kutse_App.Models
{
    public class Holiday
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sisestage püha nimi.")]
        [Display(Name = "Püha nimi")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Sisestage kuupäev.")]
        [Display(Name = "Kuupäev")]
        public DateTime Date { get; set; }
    }
}