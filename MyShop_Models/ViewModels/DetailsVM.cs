
namespace MyShop_Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();
        }

        public Product Product { get; set; }
        public bool ExitsInCart {get; set; }

        public IEnumerable<Product> SimilarProducts { get; set;}

    }
}
