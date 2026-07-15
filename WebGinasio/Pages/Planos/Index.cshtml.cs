using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Planos
{
    // Esta página apresenta a lista dos planos
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o HttpClient configurado no Program.cs
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Lista dos planos recebidos da API
        public List<PlanoViewModel> Planos { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Cria o cliente que comunica com a API
                var client = _httpClientFactory.CreateClient("API");

                // Faz um pedido GET para obter todos os planos
                var planos = await client.GetFromJsonAsync<List<PlanoViewModel>>(
                    "api/Planos"
                );

                if (planos != null)
                {
                    Planos = planos;
                }
            }
            catch
            {
                // Mostra uma mensagem simples caso a API não responda
                MensagemErro =
                    "Não foi possível carregar os planos. Verifique se a API está ligada.";
            }
        }
    }

    // Modelo usado apenas para receber os dados da API
    public class PlanoViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}