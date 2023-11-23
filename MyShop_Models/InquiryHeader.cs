using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get;set; }

        public string ApplicationUserId { get;set; }

        [ForeignKey(nameof(ApplicationUserId))] // связка через внешний ключ
        public ApplicationUser ApplicationUser { get;set;}

        public DateTime InquiryDate { get;set; }


        [Required]
        public string PhoneNumber { get;set; }
        [Required]
        public string Fullname { get;set; }
        [Required]
        public string Email { get;set; }

    }



}
