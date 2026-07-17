using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;


namespace WebGinasio.Pages.Membros
{
    [Authorize(Roles = "Administrador")]
    // Esta página apresenta os detalhes de um membro
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do membro recebido da API
        public MembroDetailsModel Membro { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura o membro através do seu ID
                var membro = await client.GetFromJsonAsync<MembroDetailsModel>(
                    $"api/Membros/{id}"
                );

                if (membro == null)
                {
                    return NotFound();
                }

                Membro = membro;
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar o membro: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para apresentar os detalhes do membro
    public class MembroDetailsModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }

        public DateTime DataInscricao { get; set; }

        public bool Ativo { get; set; }

        public int PlanoId { get; set; }

        public PlanoDetailsMembroModel? Plano { get; set; }

        public List<AulaDetailsMembroModel> Aulas { get; set; } = new();
    }

    // Modelo usado para mostrar o plano do membro
    public class PlanoDetailsMembroModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }
    }

    // Modelo usado para mostrar as aulas do membro
    public class AulaDetailsMembroModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Instrutor { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }
    }
}