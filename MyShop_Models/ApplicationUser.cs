﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop_Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Fullname { get; set; }


        [NotMapped]
        public string StreetAddress { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string State {get; set; }
        [NotMapped]
        public string PostCode { get; set; }    

       
    }
}
