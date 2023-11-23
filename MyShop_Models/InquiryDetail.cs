using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Models
{
    public class InquiryDetail
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int InquiryHeaderId { get; set; }

        [ForeignKey(nameof(InquiryHeaderId))] // связка через внешний ключ
        public InquiryHeader InquiryHeader { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))] // связка через внешний ключ
        public Product Product { get; set; }



    }
}
