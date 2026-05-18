using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ginasio.API.Data;
using Ginasio.API.Models;

namespace Ginasio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembrosController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Injeção de dependência do Contexto da Base de Dados
        public MembrosController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Membros (Listar todos os membros com o respetivo Plano)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Membro>>> GetMembros()
        {
            // Usa .Include (Eager Loading) para trazer os dados do Plano associado,
            // cumprindo o requisito de consulta de relacionamentos via LINQ.
            return await _context.Membros
                .Include(m => m.Plano)
                .ToListAsync();
        }

        // GET: api/Membros/5 (Ver detalhes de um membro e as suas aulas)
        [HttpGet("{id}")]
        public async Task<ActionResult<Membro>> GetMembro(int id)
        {
            // Traz o membro, o seu plano e a lista de aulas (Relação N:M)
            var membro = await _context.Membros
                .Include(m => m.Plano)
                .Include(m => m.Aulas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (membro == null)
            {
                return NotFound("Membro não encontrado.");
            }

            return membro;
        }

        // POST: api/Membros (Criar um novo membro)
        [HttpPost]
        public async Task<ActionResult<Membro>> PostMembro(Membro membro)
        {
            _context.Membros.Add(membro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMembro), new { id = membro.Id }, membro);
        }

        // PUT: api/Membros/5 (Editar dados do membro)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembro(int id, Membro membro)
        {
            if (id != membro.Id)
            {
                return BadRequest("O ID não coincide.");
            }

            _context.Entry(membro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Membros.Any(e => e.Id == id))
                {
                    return NotFound("O membro já não existe.");
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Membros/5 (Eliminar um membro)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembro(int id)
        {
            var membro = await _context.Membros.FindAsync(id);
            if (membro == null)
            {
                return NotFound("Membro não encontrado.");
            }

            _context.Membros.Remove(membro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}