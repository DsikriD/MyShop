using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop_Models;

namespace MyShop_DataAccess
{
    public class ApplicationDbContexts : IdentityDbContext
    {
        public ApplicationDbContexts(DbContextOptions<ApplicationDbContexts> options) : base(options)
        {
            
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<ApplicationType> ApplicationType { get; set; }

        public DbSet<Product> Product { get; set; }    

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<InquiryHeader> InquiryHeader { get; set; }

        public DbSet<InquiryDetail> InquiryDetail { get; set; }

        public DbSet<OrderHeader> OrderHeader { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }


    }
}
