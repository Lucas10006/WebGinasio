using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ginasio.API.Data;
using Ginasio.API.Models;

namespace Ginasio.API.Controllers
{
    // Controlador responsável pelas operações das aulas
    [Route("api/[controller]")]
    [ApiController]
    public class AulasController : ControllerBase
    {
        private readonly GinasioContext _context;

        // Recebe o contexto da base de dados
        public AulasController(GinasioContext context)
        {
            _context = context;
        }

        // GET: api/Aulas
        // Devolve todas as aulas e os membros inscritos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aula>>> GetAulas()
        {
            var aulas = await _context.Aulas
                .Include(a => a.Membros)
                .OrderBy(a => a.DataHora)
                .ToListAsync();

            return Ok(aulas);
        }

        // GET: api/Aulas/5
        // Devolve uma aula através do seu ID
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

            return Ok(aula);
        }

        // POST: api/Aulas
        // Cria uma nova aula
        [HttpPost]
        public async Task<ActionResult<Aula>> PostAula(Aula aula)
        {
            // Verifica se os dados recebidos são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retira espaços desnecessários
            aula.Nome = aula.Nome.Trim();
            aula.Instrutor = aula.Instrutor.Trim();

            // A aula não pode ser marcada numa data passada
            if (aula.DataHora < DateTime.Now)
            {
                return BadRequest(
                    "A data e hora da aula não podem estar no passado."
                );
            }

            // Verifica se já existe uma aula igual na mesma data
            var aulaExiste = await _context.Aulas
                .AnyAsync(a =>
                    a.Nome.ToLower() == aula.Nome.ToLower() &&
                    a.DataHora == aula.DataHora
                );

            if (aulaExiste)
            {
                return BadRequest(
                    "Já existe uma aula com esse nome nessa data e hora."
                );
            }

            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAula),
                new { id = aula.Id },
                aula
            );
        }

        // PUT: api/Aulas/5
        // Edita uma aula existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAula(int id, Aula aula)
        {
            // O ID da rota deve ser igual ao ID recebido
            if (id != aula.Id)
            {
                return BadRequest(
                    "O ID indicado não corresponde à aula."
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aulaAtual = await _context.Aulas.FindAsync(id);

            if (aulaAtual == null)
            {
                return NotFound("Aula não encontrada.");
            }

            aula.Nome = aula.Nome.Trim();
            aula.Instrutor = aula.Instrutor.Trim();

            // Verifica se outra aula já usa o mesmo nome e horário
            var aulaRepetida = await _context.Aulas
                .AnyAsync(a =>
                    a.Id != id &&
                    a.Nome.ToLower() == aula.Nome.ToLower() &&
                    a.DataHora == aula.DataHora
                );

            if (aulaRepetida)
            {
                return BadRequest(
                    "Já existe outra aula com esse nome nessa data e hora."
                );
            }

            // Não deixa reduzir a lotação abaixo do número de inscritos
            var numeroInscritos = await _context.Entry(aulaAtual)
                .Collection(a => a.Membros)
                .Query()
                .CountAsync();

            if (aula.LotacaoMaxima < numeroInscritos)
            {
                return BadRequest(
                    "A lotação máxima não pode ser inferior ao número de inscritos."
                );
            }

            // Atualiza apenas os campos necessários
            aulaAtual.Nome = aula.Nome;
            aulaAtual.Instrutor = aula.Instrutor;
            aulaAtual.DataHora = aula.DataHora;
            aulaAtual.DuracaoMinutos = aula.DuracaoMinutos;
            aulaAtual.LotacaoMaxima = aula.LotacaoMaxima;
            aulaAtual.Ativa = aula.Ativa;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Aulas/5/inscrever/3
        // Inscreve um membro numa aula
        [HttpPost("{aulaId}/inscrever/{membroId}")]
        public async Task<IActionResult> InscreverMembro(
            int aulaId,
            int membroId
        )
        {
            // Carrega a aula com os membros já inscritos
            var aula = await _context.Aulas
                .Include(a => a.Membros)
                .FirstOrDefaultAsync(a => a.Id == aulaId);

            var membro = await _context.Membros
                .FindAsync(membroId);

            if (aula == null)
            {
                return NotFound("Aula não encontrada.");
            }

            if (membro == null)
            {
                return NotFound("Membro não encontrado.");
            }

            // Apenas aulas ativas aceitam inscrições
            if (!aula.Ativa)
            {
                return BadRequest(
                    "Não é possível inscrever membros numa aula inativa."
                );
            }

            // Apenas membros ativos podem frequentar aulas
            if (!membro.Ativo)
            {
                return BadRequest(
                    "Não é possível inscrever um membro inativo."
                );
            }

            // Verifica se o membro já está inscrito
            if (aula.Membros.Any(m => m.Id == membroId))
            {
                return BadRequest(
                    "O membro já está inscrito nesta aula."
                );
            }

            // Verifica se a aula já atingiu a lotação máxima
            if (aula.Membros.Count >= aula.LotacaoMaxima)
            {
                return BadRequest(
                    "A aula já atingiu a lotação máxima."
                );
            }

            // O Entity Framework cria a ligação na tabela intermédia
            aula.Membros.Add(membro);
            await _context.SaveChangesAsync();

            return Ok(
                $"O membro '{membro.Nome}' foi inscrito na aula '{aula.Nome}'."
            );
        }

        // DELETE: api/Aulas/5/remover/3
        // Remove a inscrição de um membro numa aula
        [HttpDelete("{aulaId}/remover/{membroId}")]
        public async Task<IActionResult> RemoverMembro(
            int aulaId,
            int membroId
        )
        {
            var aula = await _context.Aulas
                .Include(a => a.Membros)
                .FirstOrDefaultAsync(a => a.Id == aulaId);

            if (aula == null)
            {
                return NotFound("Aula não encontrada.");
            }

            var membro = aula.Membros
                .FirstOrDefault(m => m.Id == membroId);

            if (membro == null)
            {
                return BadRequest(
                    "O membro não está inscrito nesta aula."
                );
            }

            // Remove apenas a ligação entre o membro e a aula
            aula.Membros.Remove(membro);
            await _context.SaveChangesAsync();

            return Ok(
                $"O membro '{membro.Nome}' foi removido da aula '{aula.Nome}'."
            );
        }

        // DELETE: api/Aulas/5
        // Elimina uma aula
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAula(int id)
        {
            var aula = await _context.Aulas
                .Include(a => a.Membros)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aula == null)
            {
                return NotFound("Aula não encontrada.");
            }

            // Remove primeiro as ligações aos membros
            aula.Membros.Clear();

            _context.Aulas.Remove(aula);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}