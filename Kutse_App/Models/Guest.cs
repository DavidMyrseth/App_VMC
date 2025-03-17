using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kutse_App.Models
{
    public class Guest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sisesta nimi")]
        [Display(Name = "Nimi")]
        public string Name { get; set; }

        [Display(Name = "E-post")]
        [Required(ErrorMessage = "Sisesta email")]
        [RegularExpression(@".+\@.+\..+", ErrorMessage = "Valesti sisestatud email")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Sisesta telefoni number")]
        [RegularExpression(@"\+372.+", ErrorMessage = "Numbri alguses peal olema +372")]
        public string Phone { get; set; }

        [Display(Name = "Kas tuleb?")]
        public bool? WillAttend { get; set; }

        [Display(Name = "Pidu")]
        public int? HolidayId { get; set; }

        public virtual Holiday Holiday { get; set; }
    }
}