using AutoMapper;
using Domain.ExternalInterfaces;
using Domain.Interfaces.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ContatoController : ControllerBase
    {
        private readonly IMapper _IMapper;
        private readonly IContato _IContato;

        public ContatoController(IMapper Imapper, IContato IContato)
        {
            _IMapper = Imapper;
            _IContato = IContato;
        }

        //[HttpGet("GetById")]
        /// <summary>
        /// Obter um contato por Id
        /// </summary>
        /// <param name="id">Identificador do contato</param>
        /// <returns>Dados do contato</returns>
        /// <response code="200">Contato consutado com sucesso</response>
        /// <response code="404">Contato não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {

            try
            {
                Contato contato = await _IContato.GetEntityById(id);
                var contatoMap = _IMapper.Map<ContatoDTO>(contato);

                if (contatoMap == null)
                {
                    return NotFound(new ResponseModelView(false, "A pesquisa não retornou resultados"));
                }

                return Ok(new ResponseModelView<ContatoDTO>(true, "Consulta realizada com sucesso", _IMapper.Map<ContatoDTO>(contatoMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Cadastrar um contato
        /// </summary>
        /// <remarks>
        /// {"id":"int", "tipo":"string", "texto":"string", "usuarioId":"int"}
        /// </remarks>
        /// <param name="contato">Dados do Contato</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Contato criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPost]
        [ProducesResponseType(typeof(ContatoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add(ContatoDTO contato)
        {

            try
            {
                
                Contato contatoMap = _IMapper.Map<Contato>(contato);
                await _IContato.Add(contatoMap);

                if (contatoMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", contatoMap.Notificacoes));
                }

                return Created("Add", new ResponseModelView<ContatoDTO>(true, "Contato adicionado com sucesso", _IMapper.Map<ContatoDTO>(contatoMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }


        }

        /// <summary>
        /// Atualizar um contato
        /// </summary>
        /// <remarks>
        /// {"id":"int", "tipo":"string", "texto":"string", "usuarioId":"int"}
        /// </remarks>
        /// <param name="id">Identificador do contato</param>
        /// <param name="contato">Dados do contato</param>
        /// <returns>Nada.</returns>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="404">Contato não encontrado</response>
        /// <response code="200">Contato alterado com sucesso</response>
        /// <response code="500">Retorna caso erros ocorram</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ContatoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(int id, ContatoDTO contato)
        {

            try
            {
                if (id != contato.Id)
                {
                    return BadRequest(new ResponseModelView(false, "Propriedade Id não pode ser alterada."));
                }

                Contato contatoMap = _IMapper.Map<Contato>(contato);
                await _IContato.Update(contatoMap);


                if (contatoMap.Notificacoes.Count > 0)
                {
                    return BadRequest(new ResponseModelView<Notifies>(false, "Falha na validação dos dados", contatoMap.Notificacoes));
                }

                return Ok(new ResponseModelView<ContatoDTO>(true, "Contato alterado com sucesso", _IMapper.Map<ContatoDTO>(contatoMap)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }

        /// <summary>
        /// Excluir um contato por Id
        /// </summary>
        /// <param name="id">Identificador do contato</param>
        /// <returns>Nada</returns>
        /// <response code="404">Contato não encontrada</response>
        /// <response code="204">Contato excluido com sucesso</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var contato = await _IContato.GetEntityById(id);
                if (contato == null)
                {
                    return NotFound(new ResponseModelView(false, "Contato não encontrado: Id "));
                }

                await _IContato.Delete(id);

                return Ok(new ResponseModelView(true, "Contato deletado com sucesso"));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelView(false, "Erro: " + ex.Message));
            }
        }


    }
}
