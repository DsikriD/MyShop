using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Models.ViewModels;
using MyShop_Utility;

namespace MyShop.Controllers
{

    [Authorize(Roles = WC.AdminRole)]

    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;

        [BindProperty]// При создании метода пост
                      // Все данные будут доступны автомотически
        public InquiryVM inquiryVM { get; set; }


        public InquiryController(IInquiryDetailRepository inqDRepo, IInquiryHeaderRepository inqHRepo)
        {
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;

        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            inquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(x => x.Id == id),
                InquiryDetail = _inqDRepo.GetAll(x => x.InquiryHeaderId == id,includeProperties:"Product"),
            };

            return View(inquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            inquiryVM.InquiryDetail = _inqDRepo.GetAll(u=>u.InquiryHeaderId == inquiryVM.InquiryHeader.Id);

            foreach(var detail in inquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            TempData[WC.Success] = "Товар добавлен в корзину";

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, inquiryVM.InquiryHeader.Id);// если Id == 0 - то не использовалась работа с inquiry
                                                                                     // если >0 - то использоваталь
            return RedirectToAction("Index","Cart");
        }


        [HttpPost]
        public ActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u=>u.Id==inquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDRepo.GetAll(u => u.InquiryHeaderId == inquiryVM.InquiryHeader.Id);

           _inqDRepo.RemoveRange(inquiryDetails);
           _inqHRepo.Remove(inquiryHeader);

            _inqHRepo.Save();// Изменения будут сохранены все поэтому можно не использовать 
                             // _inqHRepo.Save();

            return RedirectToAction(nameof(Index));

        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList() { 
            return Json(new { data = _inqHRepo.GetAll() });
        }
        #endregion
    }
}
