using System.Collections.ObjectModel;
using System.Net;
using System.Net.Mail;

namespace MyShop_Utility
{
    public static class WC
    {
        public static string ImagePath = @"\images\product\";
        public static string SessionCart = "ShoppingCartSession";
        public static string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public static string AdminEmail = "sikrierdanila@mail.ru";


        public const string CategotyName = "Category";
        public const string ApplicationTypeName= "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPeding = "Peding";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusPeding,StatusApproved,StatusInProcess,StatusShipped,StatusCancelled, StatusRefunded
            });
    
        };

    }

