using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoveMKERegistration.Models
{
    public class TShirtSizeModel
    {
        [Key]
        public string IndividualId { get; set; }
        [Required(ErrorMessage = "Shirt size is required.")]
        [Display(Name="Shirt Size")]
        public string Size { get; set; }
        [Display(Name = "First Name")]

        public string FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }
        public IEnumerable<SelectListItem> TShirtSizes { get; set; }
    }
}