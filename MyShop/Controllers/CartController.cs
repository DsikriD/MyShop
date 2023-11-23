using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop_DataAccess;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Models.ViewModels;
using MyShop_Utility;
using MyShop_Utility.BrainTree;
using NuGet.Protocol.VisualStudio;
using System.Security.Claims;
using System.Text;

namespace MyShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IOrderDetailRepository _ordDRepo;
        private readonly IOrderHeaderRepository _ordHRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContexts db, IWebHostEnvironment webHostEnvironment,IEmailSender emailSender,
            IApplicationUserRepository userRepo,IProductRepository prodRepo,IInquiryHeaderRepository inquHRepo, IInquiryDetailRepository inquDRepo,
            IOrderDetailRepository orderDetailRepo, IOrderHeaderRepository orderHeaderRepo, IBrainTreeGate brain)
        {
            //_db = db; //db - ссылка на базу данных
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _inqHRepo = inquHRepo;
            _inqDRepo = inquDRepo;
            _ordDRepo = orderDetailRepo;
            _ordHRepo = orderHeaderRepo;

            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _brain = brain;
        }



        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && // проверка на существование сессии
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()>0) // в корзине что-то есть
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            List<int> prodInCart = shoppingCartList.Select(i=>i.ProductId).ToList();
            IEnumerable<Product> productListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));// cравниваем все id с id prodInCart  
            IList<Product> productList = new List<Product>();
            // Количество для каждого товара 
            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = productListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempCount = cartObj.Count;
                productList.Add(prodTemp);
            }
            //

            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Count = prod.TempCount });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }


        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WC.AdminRole)) 
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)// Корзина заполняется на основании существующего запроса 
                {// Карзина загружена на основании запроса
                    InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u=>u.Id== HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser
                    {
                        Email = inquiryHeader.Email,
                        Fullname = inquiryHeader.Fullname,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {// Админ размещает заказ клиента,который пришел в магазин не используя сайт
                    applicationUser = new ApplicationUser();
                }

                var gateway = _brain.GetGatewat();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;

            }
            else // Когда пользователь не админ
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirst(ClaimTypes.Name);
                applicationUser = _userRepo.FirstOrDefault(u=>u.Id==claim!.Value);
            }


           

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && // проверка на существование сессии
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) // в корзине что-то есть
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodinCart = shoppingCartList.Select(i=>i.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodinCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,
                
            };

            // Передача в ProductList товара и его кол-ва
            foreach (var cartobj in shoppingCartList) {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartobj.ProductId);
                prodTemp.TempCount = cartobj.Count;
                ProductUserVM.ProductList.Add(prodTemp);
            }
            //

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM ProductUserVM)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);// Получаем id пользователя

            //var mail = Email.CreateMail("Tom", "sikrierdanila@gmail.com", 
            //    "sikrierdanila@gmail.com","subject","body");

            //Email.SendMail("smtp.gmail.com", 587, "sikrierdanila@gmail.com", "qxkpclfwlpavnpis", mail);

            //Отправка письма с заказом на почту - Начало//

            if (User.IsInRole(WC.AdminRole)) // Админ делает заказ
            {
                //var OrderTotal = 0.0;
                //foreach(Product prod in ProductUserVM.ProductList)
                //{
                //    OrderTotal += prod.Price * prod.TempCount;
                //}

                
                // Добавление заказа - Начало
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreateByUserId = claim.Value,
                    FinalOrdedTotal = ProductUserVM.ProductList.Sum(u=>u.Price * u.TempCount),
                    City = ProductUserVM.ApplicationUser.City,
                    Street = ProductUserVM.ApplicationUser.StreetAddress,
                    State = ProductUserVM.ApplicationUser.State,
                    PostCode = ProductUserVM.ApplicationUser.PostCode,
                    TransactionId = "",
                    FullName = ProductUserVM.ApplicationUser.Fullname,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPeding,
                };

                _ordHRepo.Add(orderHeader);
                _ordHRepo.Save();
                // Добавление заказа - Конец

                // Добавление деталей заказа - Начало
                foreach (var prod in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        Price = prod.Price,
                        Count = prod.TempCount,
                        ProductId = prod.Id
                    };

                    _ordDRepo.Add(orderDetail);
                }
                _ordDRepo.Save();
                // Добавление деталей заказа - Конец 

                // Создание транзакции 
                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrdedTotal/100),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };


                var gateway = _brain.GetGatewat();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }

                //сохранение деталей тразакции
                _ordHRepo.Save();
                
                return RedirectToAction(nameof(InquiryConfirmation), new {id=orderHeader.Id});

            }
            else // Пользователь отправляет запрос
            {
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString()//косая черта 
                + "inquiry.html";
                var subject = "Новый запрос";
                string HtmlBody = "";

                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                var productListSB = new StringBuilder();
                foreach (var prod in ProductUserVM.ProductList)
                {
                    productListSB.Append($"- Наименование:{prod.Name}<span style='font-size:14px;'>(ID: {prod.Id})</span><br/>");

                }

                var messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.Fullname,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());


                await _emailSender.SendEmailAsync(WC.AdminEmail, subject, messageBody);
                //Отправка письма с запросоим на почту - Конец//



                //Добовление запроса в бд - Начало//
                var inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim!.Value,
                    Fullname = ProductUserVM.ApplicationUser.Fullname,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };

                _inqHRepo.Add(inquiryHeader);
                _inqHRepo.Save();
                //Добовление запроса в бд - Конец//

                // Добавление деталей запроса - Начало
                foreach (var prod in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = prod.Id,
                    };

                    _inqDRepo.Add(inquiryDetail);
                }
                _inqDRepo.Save();
                // Добавление деталей заказа - Конец 

                TempData[WC.Success] = "Заказ успешно оформлен";
                return RedirectToAction(nameof(InquiryConfirmation));
            }
            
        }

        public IActionResult InquiryConfirmation(int id=0) {
            OrderHeader orderHeader = _ordHRepo.FirstOrDefault(u => u.Id == id);
            HttpContext.Session.Clear();// отичищаем сессию 
            return View(orderHeader);
        }



        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && // проверка на существование сессии
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) // в корзине что-то есть
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }


            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);

            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            TempData[WC.Success] = "Товар удален из корзины";
            return RedirectToAction("Index");
        }


        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            TempData[WC.Success] = "Корзина успешно отчищена";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList) 
        { 
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList) {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Count = prod.TempCount });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

    }
}
