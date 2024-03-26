using Entities.Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Token;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Cadastrar um novo usuário no Identity
        /// </summary>
        /// <remarks>
        /// {"email":"int","senha":"string","cpf":"string"}
        /// </remarks>
        /// <param name="login">Dados do usuário</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(Login), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarUsuarioIdentity([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return BadRequest("Falta alguns dados");


            var user = new ApplicationUser
            {
                UserName = login.email,
                Email = login.email,
                CPF = login.cpf,
                Tipo = TipoUsuario.Consultor,
            };

            var resultado = await _userManager.CreateAsync(user, login.senha);

            if (resultado.Errors.Any())
            {
                return Ok(resultado.Errors);
            }


            // Geração de Confirmação caso precise
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // retorno email 
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var resultado2 = await _userManager.ConfirmEmailAsync(user, code);

            if (resultado2.Succeeded)
                return Created("Usuário Adicionado com Sucesso", login);
            else
                return BadRequest("Erro ao confirmar usuários");

        }

        /// <summary>
        /// Gerar token de usuário no Identity
        /// </summary>
        /// <remarks>
        /// {"email":"int","senha":"string","cpf":"string"}
        /// </remarks>
        /// <param name="login">Dados do login</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="200">Tokn gerado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        [AllowAnonymous]
        [HttpPost("CriarTokenIdentity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelView<Notifies>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarTokenIdentity([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
            {
                return Unauthorized();
            }

            var resultado = await
                _signInManager.PasswordSignInAsync(login.email, login.senha, false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                // Recupera Usuário Logado
                var userCurrent = await _userManager.FindByEmailAsync(login.email);
                var idUsuario = userCurrent.Id;

                var token = new TokenJWTBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("Secret_Key-Gestao_Clientes-12345678"))
                    .AddSubject("Teste Gestao Clientes")
                    .AddIssuer("Teste.Security.Bearer")
                    .AddAudience("Teste.Security.Bearer")
                    .AddClaim("idUsuario", idUsuario)
                    .AddExpiry(5)
                    .Builder();

                return Ok(token.value);
            }
            else
            {
                return Unauthorized();
            }
        }
       
    }
}
