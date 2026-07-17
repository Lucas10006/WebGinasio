using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;


namespace WebGinasio.Pages.Membros
{
    [Authorize(Roles = "Administrador")]
    // Esta página permite confirmar a eliminação de um membro
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do membro que vai ser eliminado
        [BindProperty]
        public MembroDeleteModel Membro { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados do membro quando a página abre
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura o membro através do seu ID
                var membro = await client.GetFromJsonAsync<MembroDeleteModel>(
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

        // Elimina o membro depois da confirmação
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o pedido DELETE para a API
                var resposta = await client.DeleteAsync(
                    $"api/Membros/{Membro.Id}"
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                MensagemErro = await resposta.Content.ReadAsStringAsync();

                // Volta a carregar os dados caso a eliminação falhe
                var membro = await client.GetFromJsonAsync<MembroDeleteModel>(
                    $"api/Membros/{Membro.Id}"
                );

                if (membro != null)
                {
                    Membro = membro;
                }
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível eliminar o membro: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para apresentar o membro antes da eliminação
    public class MembroDeleteModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public DateTime DataInscricao { get; set; }

        public bool Ativo { get; set; }

        public PlanoDeleteMembroModel? Plano { get; set; }
    }

    // Modelo usado para mostrar o plano do membro
    public class PlanoDeleteMembroModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}