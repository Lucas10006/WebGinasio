using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;


namespace WebGinasio.Pages.Planos
{
    [Authorize(Roles = "Administrador")]
    // Esta página apresenta os detalhes de um plano
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do plano recebido da API
        public PlanoDetailsModel Plano { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura o plano pelo ID
                var plano = await client.GetFromJsonAsync<PlanoDetailsModel>(
                    $"api/Planos/{id}"
                );

                if (plano == null)
                {
                    return NotFound();
                }

                Plano = plano;
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar o plano: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para apresentar os detalhes do plano
    public class PlanoDetailsModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}