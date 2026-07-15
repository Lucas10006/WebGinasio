using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Aulas
{
    // Esta página apresenta os detalhes de uma aula
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados da aula recebidos da API
        public AulaDetailsModel Aula { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados da aula
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var carregou = await CarregarAulaAsync(id);

            if (!carregou)
            {
                return Page();
            }

            return Page();
        }

        // Remove um membro da aula
        public async Task<IActionResult> OnPostRemoverMembroAsync(
            int aulaId,
            int membroId
        )
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o pedido para remover o membro da aula
                var resposta = await client.DeleteAsync(
                    $"api/Aulas/{aulaId}/remover/{membroId}"
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage(
                        "Details",
                        new { id = aulaId }
                    );
                }

                MensagemErro = await resposta.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível remover o membro: {ex.Message}";
            }

            // Volta a carregar a aula caso aconteça algum erro
            await CarregarAulaAsync(aulaId);

            return Page();
        }

        // Método usado para evitar repetir o código de carregar a aula
        private async Task<bool> CarregarAulaAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var aula = await client.GetFromJsonAsync<AulaDetailsModel>(
                    $"api/Aulas/{id}"
                );

                if (aula == null)
                {
                    MensagemErro = "Aula não encontrada.";
                    return false;
                }

                Aula = aula;

                return true;
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar a aula: {ex.Message}";

                return false;
            }
        }
    }

    // Modelo usado para apresentar os detalhes da aula
    public class AulaDetailsModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Instrutor { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }

        public int DuracaoMinutos { get; set; }

        public int LotacaoMaxima { get; set; }

        public bool Ativa { get; set; }

        public List<MembroDetailsAulaModel> Membros { get; set; } = new();
    }

    // Modelo usado para mostrar os membros inscritos
    public class MembroDetailsAulaModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Ativo { get; set; }
    }
}