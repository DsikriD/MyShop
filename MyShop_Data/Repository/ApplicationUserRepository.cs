using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContexts _db;

        public ApplicationUserRepository(ApplicationDbContexts db) :base(db) { 
        _db = db;
        }
        
    }
}
