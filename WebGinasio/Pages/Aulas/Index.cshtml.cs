using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebGinasio.Pages.Aulas
{
    [Authorize(Roles = "Administrador")]
    // Esta página apresenta a lista das aulas
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Lista das aulas recebidas da API
        public List<AulaViewModel> Aulas { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Faz um pedido GET para obter todas as aulas
                var aulas = await client.GetFromJsonAsync<List<AulaViewModel>>(
                    "api/Aulas"
                );

                if (aulas != null)
                {
                    Aulas = aulas;
                }
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar as aulas: {ex.Message}";
            }
        }
    }

    // Modelo usado para receber os dados das aulas
    public class AulaViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Instrutor { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }

        public int DuracaoMinutos { get; set; }

        public int LotacaoMaxima { get; set; }

        public bool Ativa { get; set; }

        public List<MembroAulaViewModel> Membros { get; set; } = new();
    }

    // Modelo usado para representar os membros inscritos
    public class MembroAulaViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}