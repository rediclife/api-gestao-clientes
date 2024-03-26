using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Generics
{
    public interface IGeneric<T> where T : class
    {
        Task Add(T Obj);
        Task Update(T Obj);
        Task Delete(T Obj);
        Task Delete(int Id);
        Task<T> GetEntityById(int Id);
        Task<List<T>> List(Expression<Func<T, bool>> predicate);
        Task<List<T>> ListAll();
    }
}
