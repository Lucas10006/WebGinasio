using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Aulas
{
    [Authorize(Roles = "Administrador")]
    // Esta página permite editar os dados de uma aula
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados da aula no formulário
        [BindProperty]
        public AulaEditModel Aula { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados da aula quando a página abre
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Vai buscar a aula através do seu ID
                var aula = await client.GetFromJsonAsync<AulaEditModel>(
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

        // Envia as alterações da aula para a API
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia os dados atualizados da aula
                var resposta = await client.PutAsJsonAsync(
                    $"api/Aulas/{Aula.Id}",
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
                    $"Não foi possível editar a aula: {ex.Message}";
            }

            return Page();
        }
    }

    // Modelo usado para editar os dados da aula
    public class AulaEditModel
    {
        public int Id { get; set; }

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
        public int DuracaoMinutos { get; set; }

        [Display(Name = "Lotação Máxima")]
        [Range(
            1,
            100,
            ErrorMessage = "A lotação deve estar entre 1 e 100 pessoas."
        )]
        public int LotacaoMaxima { get; set; }

        public bool Ativa { get; set; }
    }
}