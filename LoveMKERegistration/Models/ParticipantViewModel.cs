using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoveMKERegistration.Models
{
    public class ParticipantViewModel
    {
        [Display(Name = "Group")]

        public string GroupName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Name")]
        public string DisplayName
        {
            get
            {
                string displayFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string displayLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string.Format($"{displayFirstName} {displayLastName}");

            }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}