using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MyShop_DataAccess;
using MyShop_Models;
using MyShop_Models.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using MyShop_Utility;
using MyShop_DataAccess.Repository.IRepository;

namespace MyShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _catRepo;

        public HomeController(ILogger<HomeController> logger, ICategoryRepository catRepo,
            IProductRepository prodRepo)
        {
            _logger = logger;
            _prodRepo = prodRepo;
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _prodRepo.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _catRepo.GetAll()

        };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }


            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _prodRepo.FirstOrDefault(u=>u.Id==id,includeProperties: "Category,ApplicationType"),
                ExitsInCart = false,
                SimilarProducts = _prodRepo.GetAll(u=>u.Id!=id,includeProperties: "Category,ApplicationType")

            };


            foreach(var item in ShoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExitsInCart = true;
                }
            }

            return View(detailsVM);
        }

        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
          List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            ShoppingCartList.Add(new ShoppingCart { ProductId=id, Count = detailsVM.Product.TempCount});
            HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);
            TempData[WC.Success] = "Товар добавлен в корзину";
            return RedirectToAction(nameof(Index));
        }

        //RemoveFromCart

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = ShoppingCartList.SingleOrDefault(r => r.ProductId == id);

            if (itemToRemove != null)
            {
                ShoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);
            TempData[WC.Success] = "Товар удален из корзины";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}