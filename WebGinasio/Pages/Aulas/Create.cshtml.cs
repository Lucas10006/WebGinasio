using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;   

namespace WebGinasio.Pages.Aulas
{
    [Authorize(Roles = "Administrador")]
    // Esta página permite criar uma nova aula
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
        public AulaInputModel Aula { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        public void OnGet()
        {
            // Coloca uma data inicial para facilitar o preenchimento
            Aula.DataHora = DateTime.Now.AddDays(1);
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

                // Envia a nova aula para a API
                var resposta = await client.PostAsJsonAsync(
                    "api/Aulas",
                    Aula
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                // Mostra a mensagem devolvida pela API
                MensagemErro = await resposta.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível criar a aula: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para receber os dados do formulário
    public class AulaInputModel
    {
        [Required(ErrorMessage = "O nome da aula é obrigatório.")]
        [StringLength(
            80,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 80 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do instrutor é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome do instrutor deve ter entre 3 e 100 caracteres."
        )]
        public string Instrutor { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data e hora são obrigatórias.")]
        [Display(Name = "Data e Hora")]
        public DateTime DataHora { get; set; }

        [Display(Name = "Duração")]
        [Range(
            15,
            240,
            ErrorMessage = "A duração deve estar entre 15 e 240 minutos."
        )]
        public int DuracaoMinutos { get; set; } = 60;

        [Display(Name = "Lotação Máxima")]
        [Range(
            1,
            100,
            ErrorMessage = "A lotação deve estar entre 1 e 100 pessoas."
        )]
        public int LotacaoMaxima { get; set; } = 20;

        public bool Ativa { get; set; } = true;
    }
}