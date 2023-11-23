using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace MyShop_Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required] // Поле обязательное к заполнению
        public string Name { get; set; }
    }
}
