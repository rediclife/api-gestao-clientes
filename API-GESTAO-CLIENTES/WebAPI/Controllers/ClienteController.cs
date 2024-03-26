using AutoMapper;
using Azure;
using Domain.ExternalInterfaces;
using Domain.Interfaces.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Domain.Services;
using Entities.Entities;
using Entities.ExternalEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IMapper _IMapper;
        private readonly ICliente _ICliente;
        private readonly IServiceCliente _IServiceCliente;
        private readonly IEnderecoCepExt _IAPIEnderecoCepExt;
        private readonly IEndereco _IEndereco;
        private readonly IContato _IContato;

        public ClienteController(IMapper Imapper, ICliente ICliente, IServiceCliente IServiceCliente, IEnderecoCepExt IAPIEnderecoCepExt,
                                 IEndereco IEndereco, IContato IContato)
        {
            _IMapper = Imapper;
            _ICliente = ICliente;
            _IServiceCliente = IServiceCliente;
            _IAPIEnderecoCepExt = IAPIEnderecoCepExt;
            _IEndereco = IEndereco;
            _IContato = IContato;

        }

        /// <summary>
        /// Cadastrar um cliente
        /// </summary>
        /// <remarks>
        /// {"id":"int", "nome":"string", "dataCadastro":"DateTime", {"cep":"string", "logradouro":"string", "cidade":"string", "numero":"int", "complemento":"string", "clienteId":"int"}, {["id":"int", "tipo":"string", "texto":"string"]}}
        /// </remarks>
        /// <param name="cliente">Dados do Cliente</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add(ClienteDTO cliente)
        {

            try
            {
                // Consulta API de CEP             
                EnderecoCepExt enderecoCep = _IAPIEnderecoCepExt.Consultar(cliente.Endereco.Cep);
                if (!enderecoCep.Erro)
                {
                    cliente.Endereco.Logradouro = enderecoCep.Logradouro;
                    cliente.Endereco.Cidade = enderecoCep.Localidade;
                    cliente.Endereco.Bairro = enderecoCep.Bairro;
                    cliente.Endereco.Cep = enderecoCep.Cep;
                }
                else
                {
                    return BadRequest(new ResponseModelView(false, "Cep não encontrado."));
                }

                Cliente clienteMap = _IMapper.Map<Cliente>(cliente);
                await _IServiceCliente.Adicionar(clienteMap);

                if (clienteMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", clienteMap.Notificacoes));
                }

                return Created("Add", new ResponseModelView<ClienteDTO>(true, "Cliente adicionado com sucesso", _IMapper.Map<ClienteDTO>(clienteMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }


        }

        /// <summary>
        /// Obter todos os clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="400">Retorna caso erros ocorram</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ListAll()
        {

            try
            {
                var clientes = await _ICliente.ListAll();
                var clientesMap = _IMapper.Map<List<ClienteDTO>>(clientes);

                if (clientesMap == null)
                {
                    return NotFound(new ResponseModelView(false, "A pesquisa não retornou resultados"));
                }

                return Ok(new ResponseModelView<ClienteDTO>(true, "Consulta realizada com sucesso", clientesMap));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Obter todos os clientes com Include
        /// </summary>
        /// <returns>Lista de clientes com include</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="400">Retorna caso erros ocorram</response>
        [HttpGet("ListAllInclude")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ListAllInclude()
        {

            try
            {
                var clientes = await _ICliente.ListarClientesComInclude();
                var clientesMap = _IMapper.Map<List<ClienteDTO>>(clientes);

                if (clientesMap == null)
                {
                    return NotFound(new ResponseModelView(false, "A pesquisa não retornou resultados"));
                }

                return Ok(new ResponseModelView<ClienteDTO>(true, "Consulta realizada com sucesso", clientesMap));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        //[HttpGet("GetById")]
        /// <summary>
        /// Obter um cliente por Id
        /// </summary>
        /// <param name="id">Identificador do cliente</param>
        /// <returns>Dados do cliente</returns>
        /// <response code="200">Cliente consutado com sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {

            try
            {
                Cliente cliente = await _ICliente.ObterClientePorIdComInclude(id);
                var clienteMap = _IMapper.Map<ClienteDTO>(cliente);

                if (clienteMap == null)
                {
                    return NotFound(new ResponseModelView(false, "A pesquisa não retornou resultados"));
                }

                return Ok(new ResponseModelView<ClienteDTO>(true, "Consulta realizada com sucesso", _IMapper.Map<ClienteDTO>(clienteMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }


        /// <summary>
        /// Atualizar um cliente
        /// </summary>
        /// <remarks>
        /// {"id":"int", "nome":"string", "dataCadastro":"DateTime", {"cep":"string", "logradouro":"string", "cidade":"string", "numero":"int", "complemento":"string", "clienteId":"int"}, {["id":"int", "tipo":"string", "texto":"string"]}}
        /// </remarks>
        /// <param name="id">Identificador do cliente</param>
        /// <param name="cliente">Dados do cliente</param>
        /// <returns>Nada.</returns>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="200">Cliente alterado com sucesso</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(int id, ClienteDTO cliente)
        {

            try
            {
                if (id != cliente.Id)
                {
                    return BadRequest(new ResponseModelView(false, "Propriedade Id não pode ser alterada."));
                }

                Cliente clienteMap = _IMapper.Map<Cliente>(cliente);
                await _IServiceCliente.Atualizar(clienteMap);


                if (clienteMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", clienteMap.Notificacoes));
                }

                return Ok(new ResponseModelView<ClienteDTO>(true, "Cliente alterado com sucesso", _IMapper.Map<ClienteDTO>(clienteMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Atualizar um cliente
        /// </summary>
        /// <remarks>
        /// {"id":"int", "nome":"string", "dataCadastro":"DateTime", {"cep":"string", "logradouro":"string", "cidade":"string", "numero":"int", "complemento":"string", "clienteId":"int"}, {["id":"int", "tipo":"string", "texto":"string"]}}
        /// </remarks>
        /// <param name="id">Identificador do cliente</param>
        /// <param name="clienteDto">Dados do cliente</param>
        /// <returns>Nada.</returns>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="200">Cliente alterado com sucesso</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Patch(int id, ClienteDTO clienteDto)
        {

            try
            {
                if (id != clienteDto.Id)
                {
                    return BadRequest(new ResponseModelView(false, "Propriedade Id não pode ser alterada."));
                }

                Cliente cliente = await _ICliente.ObterClientePorIdComInclude(id);
                if(cliente == null)
                {
                    return NotFound(new ResponseModelView(false, "Cliente não encontrado: Id "));
                }

                cliente.Nome = clienteDto.Nome != null ? clienteDto.Nome : cliente.Nome;
                cliente.Endereco.Numero = (int)(clienteDto.Endereco.Numero != null ? clienteDto.Endereco.Numero : cliente.Endereco.Numero);
                cliente.Endereco.Complemento = clienteDto.Endereco.Complemento != null ? clienteDto.Endereco.Complemento : cliente.Endereco.Complemento;

                if (clienteDto.Endereco.Cep != null)
                {
                    // Consulta API de CEP             
                    EnderecoCepExt enderecoCep = _IAPIEnderecoCepExt.Consultar(clienteDto.Endereco.Cep);
                    if (!enderecoCep.Erro)
                    {
                        cliente.Endereco.Logradouro = enderecoCep.Logradouro;
                        cliente.Endereco.Cidade = enderecoCep.Localidade;
                        cliente.Endereco.Bairro = enderecoCep.Bairro;
                        cliente.Endereco.Cep = enderecoCep.Cep;
                    }
                    else
                    {
                        return BadRequest(new ResponseModelView(false, "Cep não encontrado."));
                    }
                }

                if(cliente.Contatos != null && clienteDto.Contatos != null)
                {
                    for (int index = 0; index < cliente.Contatos.Count; index++)
                    {
                        for (int idx = 0; idx < clienteDto.Contatos.Count; idx++)
                        {
                            if (cliente.Contatos[index].Id == clienteDto.Contatos[idx].Id)
                            {
                                cliente.Contatos[index].Tipo = clienteDto.Contatos[idx].Tipo != null ? clienteDto.Contatos[idx].Tipo : cliente.Contatos[index].Tipo;
                                cliente.Contatos[index].Texto = clienteDto.Contatos[idx].Texto != null ? clienteDto.Contatos[idx].Texto : cliente.Contatos[index].Texto;
                            }
                        }
                    }
                }

                Cliente clienteMap = _IMapper.Map<Cliente>(cliente);
                await _ICliente.Update(clienteMap);


                if (clienteMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", clienteMap.Notificacoes));
                }

                return Ok(new ResponseModelView<ClienteDTO>(true, "Cliente alterado com sucesso", _IMapper.Map<ClienteDTO>(clienteMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Excluir um cliente por Id
        /// </summary>
        /// <param name="id">Identificador do cliente</param>
        /// <returns>Nada</returns>
        /// <response code="404">Cliente não encontrada</response>
        /// <response code="204">Cliente excluido com sucesso</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var cliente = await _ICliente.GetEntityById(id);
                if(cliente == null)
                {
                    return NotFound(new ResponseModelView(false, "Cliente não encontrado: Id "));
                }

                await _ICliente.Delete(id);

                return Ok(new ResponseModelView(true, "Cliente deletado com sucesso"));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Obter todos os endereços
        /// </summary>
        /// <returns>Lista de clientes</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="400">Retorna caso erros ocorram</response>
        [HttpGet("Endereco")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ListAllEnderecos()
        {

            try
            {
                var enderecos = await _IEndereco.ListAll();
                var enderecosMap = _IMapper.Map<List<EnderecoDTO>>(enderecos);

                if (enderecosMap == null)
                {
                    return NotFound(new ResponseModelView(false, "A pesquisa não retornou resultados"));
                }

                return Ok(new ResponseModelView<EnderecoDTO>(true, "Consulta realizada com sucesso", enderecosMap));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Atualizar um endereço
        /// </summary>
        /// <remarks>
        /// {"cep":"string", "logradouro":"string", "cidade":"string", "numero":"int", "complemento":"string", "clienteId":"int"}
        /// </remarks>
        /// <param name="id">Identificador do Endereco</param>
        /// <param name="endereco">Dados do Endereco</param>
        /// <returns>Nada.</returns>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="404">Endereco não encontrado</response>
        /// <response code="200">Endereco alterado com sucesso</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPut("Endereco/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(EnderecoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEndereco(int id, EnderecoDTO endereco)
        {

            try
            {
                if (id != endereco.Id)
                {
                    return BadRequest(new ResponseModelView(false, "Propriedade Id não pode ser alterada."));
                }

                // Consulta API de CEP             
                EnderecoCepExt enderecoCep = _IAPIEnderecoCepExt.Consultar(endereco.Cep);
                if (!enderecoCep.Erro)
                {
                    endereco.Logradouro = enderecoCep.Logradouro;
                    endereco.Cidade = enderecoCep.Localidade;
                    endereco.Bairro = enderecoCep.Bairro;
                    endereco.Cep = enderecoCep.Cep;
                }
                else
                {
                    return BadRequest(new ResponseModelView(false, "Cep não encontrado."));
                }

                Endereco enderecoMap = _IMapper.Map<Endereco>(endereco);
                await _IEndereco.Update(enderecoMap);


                if (enderecoMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", enderecoMap.Notificacoes));
                }

                return Ok(new ResponseModelView<EnderecoDTO>(true, "Endereco alterado com sucesso", _IMapper.Map<EnderecoDTO>(enderecoMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }
    }

}
