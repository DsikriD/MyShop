using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyShop_Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required] // Поле обязательное к заполнению
        public string Name { get; set; }

        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "Порядок отоброжение категории должен быть больше 0")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
