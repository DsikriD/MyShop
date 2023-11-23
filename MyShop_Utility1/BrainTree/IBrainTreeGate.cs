using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Utility.BrainTree
{
    public interface IBrainTreeGate
    {
       IBraintreeGateway CreateGateWay();
       IBraintreeGateway GetGatewat();
    }
}
