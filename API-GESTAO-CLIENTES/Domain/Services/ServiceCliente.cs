using Domain.Interfaces.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ServiceCliente : IServiceCliente
    {
        private readonly ICliente _ICliente;
        public ServiceCliente(ICliente ICliente) 
        {
            _ICliente = ICliente;
        }

        public async Task Adicionar(Cliente Objeto)
        {
            try
            {
                await _ICliente.Add(Objeto);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        public async Task Atualizar(Cliente Objeto)
        {
            try
            {
                var validaDescricao = Objeto.ValidarPropriedadeString(Objeto.Nome, "Descricao");
                if (validaDescricao)
                {
                    //var lstClientes = await _ICliente.ListarClientes(p => p.Nome == Objeto.Nome);
                    //if (lstClientes.Count > 0)
                    //{
                    //    Objeto.AdicionarNotificacao("O cliente já existe na base de dados.", "CodigoEan");
                    //}
                    //else
                    //{
                    //    await _ICliente.Update(Objeto);
                    //}

                    await _ICliente.Update(Objeto);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

    }
}
