using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Models.ViewModels;
using MyShop_Utility;
using MyShop_Utility.BrainTree;
using System.Transactions;

namespace MyShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _ordHRepo;
        private readonly IOrderDetailRepository _ordDRepo;
        private readonly IBrainTreeGate _brain;


        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository ordHRepo, IOrderDetailRepository ordDRepo, IBrainTreeGate brain)
        {
            _ordHRepo = ordHRepo;
            _ordDRepo = ordDRepo;
            _brain = brain;
        }




        public IActionResult Index(string searchName=null,string searchEmail = null,string searchPhone= null,string Status=null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderList = _ordHRepo.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i=>new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text =i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status)&& Status!= "--Статус заказа--")
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }


            return View(orderListVM);
        }

        public IActionResult Details(int id) {

            OrderVM orderVM = new OrderVM {
                OrderHeader = _ordHRepo.FirstOrDefault(u => u.Id == id),
                OrderDetails = _ordDRepo.GetAll(u => u.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(orderVM);
        }


        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _ordHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            TempData[WC.Success] = "Заказ начат";
            _ordHRepo.Save();
            return RedirectToAction(nameof(Index));


        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _ordHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShoppingDate = DateTime.Now;
            _ordHRepo.Save();
            TempData[WC.Success] = "Заказ успешно отправлен";
            return RedirectToAction(nameof(Index));


        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _ordHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            var gareway = _brain.GetGatewat();// шлюз для совершения транзакция 
            Braintree.Transaction transaction = gareway.Transaction.Find(orderHeader.TransactionId);

            if(transaction.Status == Braintree.TransactionStatus.AUTHORIZED || transaction.Status == Braintree.TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {//деньги еще не переведены , в этом случае не нужно возвращать деньги
                Braintree.Result<Braintree.Transaction> resultvoid = gareway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {//деньги переведены
                Braintree.Result<Braintree.Transaction> resultfund = gareway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WC.StatusRefunded;

            _ordHRepo.Save();
            TempData[WC.Success] = "Заказ был успешно отменен";
            return RedirectToAction(nameof(Index));

        }


        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _ordHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.Street = OrderVM.OrderHeader.Street;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostCode = OrderVM.OrderHeader.PostCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;


            _ordHRepo.Save();
            TempData[WC.Success] = "Детали заказа были успешно обновлены";

            return RedirectToAction("Details","Order",new {id=orderHeaderFromDb.Id});
        }


    }
}
