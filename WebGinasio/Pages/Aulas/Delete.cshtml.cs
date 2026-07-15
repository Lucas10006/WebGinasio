using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Aulas
{
    // Esta página permite confirmar a eliminação de uma aula
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados da aula que vai ser eliminada
        [BindProperty]
        public AulaDeleteModel Aula { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados da aula quando a página abre
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura a aula através do seu ID
                var aula = await client.GetFromJsonAsync<AulaDeleteModel>(
                    $"api/Aulas/{id}"
                );

                if (aula == null)
                {
                    return NotFound();
                }

                Aula = aula;
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar a aula: {ex.Message}";
            }

            return Page();
        }

        // Elimina a aula depois da confirmação
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o pedido DELETE para a API
                var resposta = await client.DeleteAsync(
                    $"api/Aulas/{Aula.Id}"
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                // Mostra a mensagem devolvida pela API
                MensagemErro = await resposta.Content.ReadAsStringAsync();

                // Volta a carregar os dados caso a eliminação falhe
                var aula = await client.GetFromJsonAsync<AulaDeleteModel>(
                    $"api/Aulas/{Aula.Id}"
                );

                if (aula != null)
                {
                    Aula = aula;
                }
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível eliminar a aula: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para apresentar a aula antes da eliminação
    public class AulaDeleteModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Instrutor { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }

        public int DuracaoMinutos { get; set; }

        public int LotacaoMaxima { get; set; }

        public bool Ativa { get; set; }

        public List<MembroDeleteAulaModel> Membros { get; set; } = new();
    }

    // Modelo usado para representar os membros inscritos
    public class MembroDeleteAulaModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}