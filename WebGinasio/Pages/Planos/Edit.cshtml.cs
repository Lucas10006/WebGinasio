using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebGinasio.Pages.Planos
{
    [Authorize(Roles = "Administrador")]
    // Esta página permite editar um plano
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do plano no formulário
        [BindProperty]
        public PlanoEditModel Plano { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados do plano quando a página abre
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Procura o plano através do seu ID
                var plano = await client.GetFromJsonAsync<PlanoEditModel>(
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

        // Envia as alterações quando o formulário é submetido
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia os dados atualizados para a API
                var resposta = await client.PutAsJsonAsync(
                    $"api/Planos/{Plano.Id}",
                    Plano
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                MensagemErro = await resposta.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível editar o plano: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para editar o plano
    public class PlanoEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do plano é obrigatório.")]
        [StringLength(
            60,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 60 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [StringLength(
            300,
            ErrorMessage = "A descrição não pode ter mais de 300 caracteres."
        )]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(
            0.01,
            1000,
            ErrorMessage = "O preço deve estar entre 0,01€ e 1000€."
        )]
        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}