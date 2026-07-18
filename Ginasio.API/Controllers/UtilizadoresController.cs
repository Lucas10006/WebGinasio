using Ginasio.API.Data;
using Ginasio.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Controllers
{
    // Controlador responsável pelas contas dos utilizadores
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadoresController : ControllerBase
    {
        private readonly GinasioContext _context;
        private readonly IPasswordHasher<Utilizador> _passwordHasher;

        // Recebe o contexto e o serviço que protege as palavras-passe
        public UtilizadoresController(
            GinasioContext context,
            IPasswordHasher<Utilizador> passwordHasher
        )
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Utilizadores
        // Devolve todas as contas sem mostrar as palavras-passe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilizadorResposta>>> GetUtilizadores()
        {
            var utilizadores = await _context.Utilizadores
                .Include(u => u.Membro)
                .OrderBy(u => u.Nome)
                .Select(u => new UtilizadorResposta
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    TipoUtilizador = u.TipoUtilizador,
                    Ativo = u.Ativo,
                    MembroId = u.MembroId,
                    NomeMembro = u.Membro != null
                        ? u.Membro.Nome
                        : null
                })
                .ToListAsync();

            return Ok(utilizadores);
        }

        // GET: api/Utilizadores/5
        // Devolve uma conta através do seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UtilizadorResposta>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores
                .Include(u => u.Membro)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var resposta = new UtilizadorResposta
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Email = utilizador.Email,
                TipoUtilizador = utilizador.TipoUtilizador,
                Ativo = utilizador.Ativo,
                MembroId = utilizador.MembroId,
                NomeMembro = utilizador.Membro?.Nome
            };

            return Ok(resposta);
        }

        // POST: api/Utilizadores
        // Cria uma nova conta de utilizador
        [HttpPost]
        public async Task<ActionResult<UtilizadorResposta>> PostUtilizador(
            CriarUtilizadorPedido pedido
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pedido.Nome = pedido.Nome.Trim();
            pedido.Email = pedido.Email.Trim().ToLower();
            pedido.TipoUtilizador = pedido.TipoUtilizador.Trim();

            // Apenas existem estes dois tipos de conta
            if (
                pedido.TipoUtilizador != "Administrador" &&
                pedido.TipoUtilizador != "Membro"
            )
            {
                return BadRequest(
                    "O tipo de utilizador deve ser Administrador ou Membro."
                );
            }

            // Verifica se o email já está a ser usado
            var emailExiste = await _context.Utilizadores
                .AnyAsync(u => u.Email.ToLower() == pedido.Email);

            if (emailExiste)
            {
                return BadRequest(
                    "Já existe uma conta com esse email."
                );
            }

            // Uma conta de membro tem de estar ligada a um membro
            if (pedido.TipoUtilizador == "Membro")
            {
                if (!pedido.MembroId.HasValue)
                {
                    return BadRequest(
                        "Uma conta de membro deve estar associada a um membro."
                    );
                }

                var membro = await _context.Membros
                    .FirstOrDefaultAsync(m => m.Id == pedido.MembroId.Value);

                if (membro == null)
                {
                    return BadRequest(
                        "O membro escolhido não existe."
                    );
                }

                // Impede duas contas associadas ao mesmo membro
                var membroJaTemConta = await _context.Utilizadores
                    .AnyAsync(u => u.MembroId == pedido.MembroId);

                if (membroJaTemConta)
                {
                    return BadRequest(
                        "O membro escolhido já tem uma conta."
                    );
                }
            }
            else
            {
                // Um administrador não precisa de estar ligado a um membro
                pedido.MembroId = null;
            }

            var utilizador = new Utilizador
            {
                Nome = pedido.Nome,
                Email = pedido.Email,
                TipoUtilizador = pedido.TipoUtilizador,
                Ativo = pedido.Ativo,
                MembroId = pedido.MembroId
            };

            // Protege a palavra-passe antes de a guardar
            utilizador.PasswordHash = _passwordHasher.HashPassword(
                utilizador,
                pedido.Password
            );

            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();

            var resposta = new UtilizadorResposta
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Email = utilizador.Email,
                TipoUtilizador = utilizador.TipoUtilizador,
                Ativo = utilizador.Ativo,
                MembroId = utilizador.MembroId
            };

            return CreatedAtAction(
                nameof(GetUtilizador),
                new { id = utilizador.Id },
                resposta
            );
        }

        // PUT: api/Utilizadores/5
        // Edita os dados principais de uma conta
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(
            int id,
            EditarUtilizadorPedido pedido
        )
        {
            if (id != pedido.Id)
            {
                return BadRequest(
                    "O ID indicado não corresponde ao utilizador."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var utilizador = await _context.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            pedido.Nome = pedido.Nome.Trim();
            pedido.Email = pedido.Email.Trim().ToLower();
            pedido.TipoUtilizador = pedido.TipoUtilizador.Trim();

            if (
                pedido.TipoUtilizador != "Administrador" &&
                pedido.TipoUtilizador != "Membro"
            )
            {
                return BadRequest(
                    "O tipo de utilizador deve ser Administrador ou Membro."
                );
            }

            var emailRepetido = await _context.Utilizadores
                .AnyAsync(u =>
                    u.Id != id &&
                    u.Email.ToLower() == pedido.Email
                );

            if (emailRepetido)
            {
                return BadRequest(
                    "Já existe outra conta com esse email."
                );
            }

            if (pedido.TipoUtilizador == "Membro")
            {
                if (!pedido.MembroId.HasValue)
                {
                    return BadRequest(
                        "Uma conta de membro deve estar associada a um membro."
                    );
                }

                var membroExiste = await _context.Membros
                    .AnyAsync(m => m.Id == pedido.MembroId.Value);

                if (!membroExiste)
                {
                    return BadRequest(
                        "O membro escolhido não existe."
                    );
                }

                var membroJaTemConta = await _context.Utilizadores
                    .AnyAsync(u =>
                        u.Id != id &&
                        u.MembroId == pedido.MembroId
                    );

                if (membroJaTemConta)
                {
                    return BadRequest(
                        "O membro escolhido já tem outra conta."
                    );
                }
            }
            else
            {
                pedido.MembroId = null;
            }

            utilizador.Nome = pedido.Nome;
            utilizador.Email = pedido.Email;
            utilizador.TipoUtilizador = pedido.TipoUtilizador;
            utilizador.Ativo = pedido.Ativo;
            utilizador.MembroId = pedido.MembroId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Utilizadores/5
        // Elimina uma conta de utilizador
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            _context.Utilizadores.Remove(utilizador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Dados recebidos para criar uma conta
    public class CriarUtilizadorPedido
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "A palavra-passe deve ter pelo menos 6 caracteres."
        )]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        public string TipoUtilizador { get; set; } = "Membro";

        public bool Ativo { get; set; } = true;

        public int? MembroId { get; set; }
    }

    // Dados recebidos para editar uma conta
    public class EditarUtilizadorPedido
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        public string TipoUtilizador { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        public int? MembroId { get; set; }
    }

    // Dados devolvidos pela API
    public class UtilizadorResposta
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TipoUtilizador { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        public int? MembroId { get; set; }

        public string? NomeMembro { get; set; }
    }
}