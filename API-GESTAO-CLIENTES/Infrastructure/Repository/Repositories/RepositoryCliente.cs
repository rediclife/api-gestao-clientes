using Domain.Interfaces;
using Domain.Interfaces.Interfaces;
using Entities.Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Repositories
{
    public class RepositoryCliente : RepositoyGenerics<Cliente>, ICliente
    {
        private readonly DbContextOptions<ContextBase> _OptionsBuilder;
        public RepositoryCliente()
        {
            _OptionsBuilder = new DbContextOptions<ContextBase>();
        }

        public async Task<List<Cliente>> ListarClientesPred(Expression<Func<Cliente, bool>> predicate)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await banco.Cliente.Where(predicate).AsNoTracking().ToListAsync();
            }
        }

        public async Task<ICollection<Cliente>> ListarClientesComInclude()
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await banco.Cliente.Include(e => e.Endereco).Include(c => c.Contatos).ToListAsync();
            }
        }

        public async Task<Cliente> ObterClientePorIdComInclude(int Id)
        {
            using (var banco = new ContextBase(_OptionsBuilder))
            {
                return await banco.Cliente.Where(c => c.Id == Id).Include(e => e.Endereco).Include(c => c.Contatos).FirstOrDefaultAsync();
            }
        }
    }
}
