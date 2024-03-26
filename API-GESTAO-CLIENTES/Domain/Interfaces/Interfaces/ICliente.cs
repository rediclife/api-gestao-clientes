using Domain.Interfaces.Generics;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Interfaces
{
    public interface ICliente : IGeneric<Cliente>
    {
        Task<List<Cliente>> ListarClientesPred(Expression<Func<Cliente, bool>> predicate);
        Task<ICollection<Cliente>> ListarClientesComInclude();
        Task<Cliente> ObterClientePorIdComInclude(int Id);
    }
}
