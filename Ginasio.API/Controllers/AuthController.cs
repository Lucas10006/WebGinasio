using Ginasio.API.Data;
using Ginasio.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Controllers
{
    // Controlador responsável pelo login
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GinasioContext _context;
        private readonly IPasswordHasher<Utilizador> _passwordHasher;

        public AuthController(
            GinasioContext context,
            IPasswordHasher<Utilizador> passwordHasher
        )
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResposta>> Login(
            LoginPedido pedido
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = pedido.Email.Trim().ToLower();

            // Procura o utilizador pelo email
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (utilizador == null)
            {
                return Unauthorized("Email ou palavra-passe inválidos.");
            }

            // Impede o login de contas desativadas
            if (!utilizador.Ativo)
            {
                return Unauthorized("Esta conta está desativada.");
            }

            // Verifica se a palavra-passe corresponde ao hash guardado
            var resultado = _passwordHasher.VerifyHashedPassword(
                utilizador,
                utilizador.PasswordHash,
                pedido.Password
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Email ou palavra-passe inválidos.");
            }

            var resposta = new LoginResposta
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Email = utilizador.Email,
                TipoUtilizador = utilizador.TipoUtilizador,
                MembroId = utilizador.MembroId
            };

            return Ok(resposta);
        }
    }

    // Dados recebidos no login
    public class LoginPedido
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        public string Password { get; set; } = string.Empty;
    }

    // Dados devolvidos depois de um login válido
    public class LoginResposta
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TipoUtilizador { get; set; } = string.Empty;

        public int? MembroId { get; set; }
    }
}