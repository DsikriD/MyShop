using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _options { get; set; }
        private IBraintreeGateway braintreeGateway { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateWay()
        {
            return new BraintreeGateway(_options.Enviroment,_options.MerchantId,_options.PublicKey,_options.PrivateKey);
        }

        public IBraintreeGateway GetGatewat()
        {
            if(braintreeGateway == null)
            {
                braintreeGateway = CreateGateWay();
            }
            return braintreeGateway;    
        }
    }
}
