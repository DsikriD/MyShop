using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string CreateByUserId { get; set; }
        [ForeignKey("CreateByUserId")]

        public ApplicationUser CreteBy { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime ShoppingDate { get; set; }

        [Required]
        public double FinalOrdedTotal { get; set; }

        public string OrderStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public string TransactionId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostCode { get; set; }

        public string FullName { get; set; }    

        public string  Email { get; set; }

    }
}
