using Ginasio.API.Data;
using Ginasio.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ginasio.API.Controllers
{
    // Controlador responsável pelas operações dos planos
    [Route("api/[controller]")]
    [ApiController]
    public class PlanosController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Recebe o contexto da base de dados
        public PlanosController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Planos
        // Devolve todos os planos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plano>>> GetPlanos()
        {
            var planos = await _context.Planos
                .OrderBy(p => p.Nome)
                .ToListAsync();

            return Ok(planos);
        }

        // GET: api/Planos/5
        // Devolve um plano através do seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Plano>> GetPlano(int id)
        {
            var plano = await _context.Planos
                .Include(p => p.Membros)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plano == null)
            {
                return NotFound("Plano não encontrado.");
            }

            return Ok(plano);
        }

        // POST: api/Planos
        // Cria um novo plano
        [HttpPost]
        public async Task<ActionResult<Plano>> PostPlano(Plano plano)
        {
            // Verifica se os dados recebidos são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retira espaços no início e no fim do nome
            plano.Nome = plano.Nome.Trim();

            // Verifica se já existe um plano com o mesmo nome
            var planoExiste = await _context.Planos
                .AnyAsync(p => p.Nome.ToLower() == plano.Nome.ToLower());

            if (planoExiste)
            {
                return BadRequest("Já existe um plano com esse nome.");
            }

            _context.Planos.Add(plano);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPlano),
                new { id = plano.Id },
                plano
            );
        }

        // PUT: api/Planos/5
        // Edita um plano existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlano(int id, Plano plano)
        {
            // O ID da rota tem de ser igual ao ID do objeto
            if (id != plano.Id)
            {
                return BadRequest("O ID indicado não corresponde ao plano.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var planoAtual = await _context.Planos.FindAsync(id);

            if (planoAtual == null)
            {
                return NotFound("Plano não encontrado.");
            }

            plano.Nome = plano.Nome.Trim();

            // Verifica se outro plano já usa o mesmo nome
            var nomeRepetido = await _context.Planos
                .AnyAsync(p =>
                    p.Id != id &&
                    p.Nome.ToLower() == plano.Nome.ToLower()
                );

            if (nomeRepetido)
            {
                return BadRequest("Já existe outro plano com esse nome.");
            }

            // Atualiza apenas os campos necessários
            planoAtual.Nome = plano.Nome;
            planoAtual.Descricao = plano.Descricao;
            planoAtual.Preco = plano.Preco;
            planoAtual.Ativo = plano.Ativo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Planos/5
        // Elimina um plano se não tiver membros associados
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlano(int id)
        {
            var plano = await _context.Planos
                .Include(p => p.Membros)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plano == null)
            {
                return NotFound("Plano não encontrado.");
            }

            // Um plano com membros associados não pode ser eliminado
            if (plano.Membros.Any())
            {
                return BadRequest(
                    "Não é possível eliminar um plano que tenha membros associados."
                );
            }

            _context.Planos.Remove(plano);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}