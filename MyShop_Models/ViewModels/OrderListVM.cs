﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status {get; set;}

    }
}
