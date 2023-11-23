using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class //where T : class - означает что Т наследуется от любого класса
    {
        T Find(int id);

        IEnumerable<T> GetAll(
            Expression<Func <T, bool>> filter = null,// для where
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, // для .include
            bool isTracking = true // отслеживание запроса, если false - отключает редактирование данных
                                                            // и запрос не отслеживается (тратится меньше ресурсов)
            );

        T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true
            );

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

        void Save();
    }
}
