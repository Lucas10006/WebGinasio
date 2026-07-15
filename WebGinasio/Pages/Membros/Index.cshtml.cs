using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Membros
{
    // Esta página apresenta a lista dos membros
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Lista dos membros recebidos da API
        public List<MembroViewModel> Membros { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Faz um pedido GET para obter todos os membros
                var membros = await client.GetFromJsonAsync<List<MembroViewModel>>(
                    "api/Membros"
                );

                if (membros != null)
                {
                    Membros = membros;
                }
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar os membros: {ex.Message}";
            }
        }
    }

    // Modelo usado para receber os dados dos membros
    public class MembroViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }

        public DateTime DataInscricao { get; set; }

        public bool Ativo { get; set; }

        public int PlanoId { get; set; }

        public PlanoMembroViewModel? Plano { get; set; }
    }

    // Modelo usado para mostrar o plano do membro
    public class PlanoMembroViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}