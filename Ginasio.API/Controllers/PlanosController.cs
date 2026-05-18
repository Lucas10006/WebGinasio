using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ginasio.API.Data;
using Ginasio.API.Models;

namespace Ginasio.API.Controllers
{
    // Define a rota da API (ex: api/planos) e indica que é um controlador de API
    [Route("api/[controller]")]
    [ApiController]
    public class PlanosController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Injeção de dependência do Contexto da Base de Dados
        public PlanosController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Planos (Listar todos os planos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plano>>> GetPlanos()
        {
            // Usa LINQ de forma assíncrona para obter todos os planos da tabela
            return await _context.Planos.ToListAsync();
        }

        // GET: api/Planos/5 (Ver detalhes de um plano específico)
        [HttpGet("{id}")]
        public async Task<ActionResult<Plano>> GetPlano(int id)
        {
            // Procura o plano pelo ID enviado na rota
            var plano = await _context.Planos.FindAsync(id);

            if (plano == null)
            {
                // Retorna um erro HTTP 404 se não for encontrado
                return NotFound("Plano não encontrado.");
            }

            return plano;
        }

        // POST: api/Planos (Criar um novo plano)
        [HttpPost]
        public async Task<ActionResult<Plano>> PostPlano(Plano plano)
        {
            // Adiciona o novo objeto à tabela de Planos
            _context.Planos.Add(plano);
            // Guarda as alterações na Base de Dados de forma definitiva
            await _context.SaveChangesAsync();

            // Retorna o status HTTP 201 (Created) com a localização do novo recurso
            return CreatedAtAction(nameof(GetPlano), new { id = plano.Id }, plano);
        }

        // PUT: api/Planos/5 (Atualizar/Editar um plano completo)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlano(int id, Plano plano)
        {
            if (id != plano.Id)
            {
                return BadRequest("O ID fornecido não coincide.");
            }

            // Informa o Entity Framework que o objeto foi modificado
            _context.Entry(plano).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Planos.Any(e => e.Id == id))
                {
                    return NotFound("O plano já não existe.");
                }
                throw;
            }

            return NoContent(); // HTTP 204 (Sucesso, sem conteúdo para retornar)
        }

        // DELETE: api/Planos/5 (Eliminar um plano)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlano(int id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano == null)
            {
                return NotFound("Plano não encontrado.");
            }

            // Remove o registo e guarda as alterações
            _context.Planos.Remove(plano);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}