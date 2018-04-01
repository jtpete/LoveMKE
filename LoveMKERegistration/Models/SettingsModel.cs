using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace LoveMKERegistration.Models
{
    public class SettingsModel
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Has T-Shirt Size Signups")]
        public bool HasTShirts { get; set; }

        public byte[] Logo { get; set; }

        public string LogoName { get; set; }


        [Display(Name = "Background Image")]
        public byte[] Background { get; set; }

        public string BackgroundName { get; set; }


    }
}