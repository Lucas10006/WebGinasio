using Ginasio.API.Data;
using Ginasio.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ginasio.API.Controllers
{
    // Controlador responsável pelas operações dos membros
    [Route("api/[controller]")]
    [ApiController]
    public class MembrosController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Recebe o contexto da base de dados
        public MembrosController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Membros
        // Devolve todos os membros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Membro>>> GetMembros()
        {
            var membros = await _context.Membros
                .Include(m => m.Plano)
                .Include(m => m.Aulas)
                .OrderBy(m => m.Nome)
                .ToListAsync();

            return Ok(membros);
        }

        // GET: api/Membros/5
        // Devolve um membro através do seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Membro>> GetMembro(int id)
        {
            var membro = await _context.Membros
                .Include(m => m.Plano)
                .Include(m => m.Aulas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (membro == null)
            {
                return NotFound("Membro não encontrado.");
            }

            return Ok(membro);
        }

        // POST: api/Membros
        // Cria um novo membro
        [HttpPost]
        public async Task<ActionResult<Membro>> PostMembro(Membro membro)
        {
            // Verifica se os dados recebidos são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retira espaços desnecessários
            membro.Nome = membro.Nome.Trim();
            membro.Email = membro.Email.Trim().ToLower();

            // Verifica se o plano escolhido existe
            var planoExiste = await _context.Planos
                .AnyAsync(p => p.Id == membro.PlanoId);

            if (!planoExiste)
            {
                return BadRequest("O plano escolhido não existe.");
            }

            // Verifica se já existe um membro com o mesmo email
            var emailExiste = await _context.Membros
                .AnyAsync(m => m.Email.ToLower() == membro.Email);

            if (emailExiste)
            {
                return BadRequest("Já existe um membro com esse email.");
            }

            // Impede datas de nascimento no futuro
            if (
                membro.DataNascimento.HasValue &&
                membro.DataNascimento.Value.Date > DateTime.Today
            )
            {
                return BadRequest(
                    "A data de nascimento não pode ser uma data futura."
                );
            }

            // A data de inscrição é definida no momento do registo
            membro.DataInscricao = DateTime.Now;

            _context.Membros.Add(membro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMembro),
                new { id = membro.Id },
                membro
            );
        }

        // PUT: api/Membros/5
        // Edita um membro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembro(int id, Membro membro)
        {
            // O ID da rota tem de ser igual ao ID recebido
            if (id != membro.Id)
            {
                return BadRequest("O ID indicado não corresponde ao membro.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var membroAtual = await _context.Membros.FindAsync(id);

            if (membroAtual == null)
            {
                return NotFound("Membro não encontrado.");
            }

            membro.Nome = membro.Nome.Trim();
            membro.Email = membro.Email.Trim().ToLower();

            // Verifica se o novo plano existe
            var planoExiste = await _context.Planos
                .AnyAsync(p => p.Id == membro.PlanoId);

            if (!planoExiste)
            {
                return BadRequest("O plano escolhido não existe.");
            }

            // Verifica se outro membro já usa o mesmo email
            var emailRepetido = await _context.Membros
                .AnyAsync(m =>
                    m.Id != id &&
                    m.Email.ToLower() == membro.Email
                );

            if (emailRepetido)
            {
                return BadRequest(
                    "Já existe outro membro com esse email."
                );
            }

            if (
                membro.DataNascimento.HasValue &&
                membro.DataNascimento.Value.Date > DateTime.Today
            )
            {
                return BadRequest(
                    "A data de nascimento não pode ser uma data futura."
                );
            }

            // Atualiza apenas os campos que podem ser alterados
            membroAtual.Nome = membro.Nome;
            membroAtual.Email = membro.Email;
            membroAtual.Telefone = membro.Telefone;
            membroAtual.DataNascimento = membro.DataNascimento;
            membroAtual.Ativo = membro.Ativo;
            membroAtual.PlanoId = membro.PlanoId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Membros/5
        // Elimina um membro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembro(int id)
        {
            var membro = await _context.Membros
                .Include(m => m.Aulas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (membro == null)
            {
                return NotFound("Membro não encontrado.");
            }

            // Remove primeiro as ligações do membro às aulas
            membro.Aulas.Clear();

            _context.Membros.Remove(membro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}