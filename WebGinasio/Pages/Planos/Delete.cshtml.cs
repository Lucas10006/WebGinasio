using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebGinasio.Pages.Planos
{
    [Authorize(Roles = "Administrador")]
    // Esta página permite confirmar a eliminação de um plano
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do plano que vai ser eliminado
        [BindProperty]
        public PlanoDeleteModel Plano { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados do plano quando a página abre
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura o plano através do seu ID
                var plano = await client.GetFromJsonAsync<PlanoDeleteModel>(
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

        // Elimina o plano depois da confirmação
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o pedido DELETE para a API
                var resposta = await client.DeleteAsync(
                    $"api/Planos/{Plano.Id}"
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                // Mostra a mensagem enviada pela API
                MensagemErro = await resposta.Content.ReadAsStringAsync();

                // Volta a carregar o plano caso a eliminação falhe
                var plano = await client.GetFromJsonAsync<PlanoDeleteModel>(
                    $"api/Planos/{Plano.Id}"
                );

                if (plano != null)
                {
                    Plano = plano;
                }
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível eliminar o plano: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para apresentar o plano antes da eliminação
    public class PlanoDeleteModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}