using MyShop_Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop_Models
{
    public class Product
    {
        public Product()
        {
            TempCount = 1;
        }


        [Key]
        public int Id { get; set; }


        public string Name { get; set; }

        public string ShortDes { get; set; }

        public string Description { get; set;}

        [Range(1, int.MaxValue)]
        public double Price { get; set; }

        public  string Image{ get; set; }

        [Display(Name="Category Type")]
        public  int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Application Type")]
        public int ApplicationTypeId { get; set; }

        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }


        [NotMapped] // не добалвять в бд 
        [Range(1,100, ErrorMessage = "Количество должно быть больше 0")]
        public int TempCount { get; set; } 
    }
}



