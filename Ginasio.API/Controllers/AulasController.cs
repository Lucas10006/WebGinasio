using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ginasio.API.Data;
using Ginasio.API.Models;

namespace Ginasio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AulasController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Injeção de dependência do Contexto da Base de Dados
        public AulasController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Aulas (Listar todas as aulas e os membros inscritos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aula>>> GetAulas()
        {
            // Usa .Include para trazer a lista de membros de cada aula (Muitos-para-Muitos)
            return await _context.Aulas
                .Include(a => a.Membros)
                .ToListAsync();
        }

        // GET: api/Aulas/5 (Ver detalhes de uma aula específica)
        [HttpGet("{id}")]
        public async Task<ActionResult<Aula>> GetAula(int id)
        {
            var aula = await _context.Aulas
                .Include(a => a.Membros)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aula == null)
            {
                return NotFound("Aula não encontrada.");
            }

            return aula;
        }

        // POST: api/Aulas (Criar uma nova aula/modalidade)
        [HttpPost]
        public async Task<ActionResult<Aula>> PostAula(Aula aula)
        {
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAula), new { id = aula.Id }, aula);
        }

        // POST: api/Aulas/Inscrever (Inscrever um Membro numa Aula - Relação N:M)
        [HttpPost("Inscrever")]
        public async Task<IActionResult> InscreverMembro(int aulaId, int membroId)
        {
            // Carrega a aula com os seus membros e o membro que se quer inscrever
            var aula = await _context.Aulas.Include(a => a.Membros).FirstOrDefaultAsync(a => a.Id == aulaId);
            var membro = await _context.Membros.FindAsync(membroId);

            if (aula == null || membro == null)
            {
                return NotFound("Aula ou Membro não encontrado.");
            }

            // Verifica se o membro já está inscrito para evitar duplicados
            if (aula.Membros.Any(m => m.Id == membroId))
            {
                return BadRequest("O membro já está inscrito nesta aula.");
            }

            // O Entity Framework Core deteta a relação N:M e insere na tabela intermédia sozinho!
            aula.Membros.Add(membro);
            await _context.SaveChangesAsync();

            return Ok($"Membro '{membro.Nome}' inscrito com sucesso na aula de '{aula.Nome}'!");
        }

        // DELETE: api/Aulas/5 (Eliminar uma aula)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAula(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null)
            {
                return NotFound("Aula não encontrada.");
            }

            _context.Aulas.Remove(aula);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}