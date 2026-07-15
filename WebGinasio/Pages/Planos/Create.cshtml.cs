using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Planos
{
    // Esta página permite criar um novo plano
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados preenchidos no formulário
        [BindProperty]
        public PlanoInputModel Plano { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Verifica se os dados do formulário são válidos
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia os dados para a API
                var resposta = await client.PostAsJsonAsync(
                    "api/Planos",
                    Plano
                );

                if (resposta.IsSuccessStatusCode)
                {
                    // Depois de criar, volta para a lista de planos
                    return RedirectToPage("Index");
                }

                // Lê a mensagem enviada pela API
                MensagemErro = await resposta.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível criar o plano: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para receber os dados do formulário
    public class PlanoInputModel
    {
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

        public bool Ativo { get; set; } = true;
    }
}