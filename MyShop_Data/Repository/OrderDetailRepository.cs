using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContexts _db;

        public OrderDetailRepository(ApplicationDbContexts db) :base(db) { 
        _db = db;
        }


        public void Update(OrderDetail obj)
        {
            _db.OrderDetail.Update(obj);
        }
    }
}
