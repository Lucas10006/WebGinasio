using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Membros
{
    // Esta página permite criar um novo membro
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
        public MembroInputModel Membro { get; set; } = new();

        // Lista usada na caixa de seleção dos planos
        public List<SelectListItem> Planos { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os planos quando a página abre
        public async Task OnGetAsync()
        {
            await CarregarPlanosAsync();
        }

        // Envia os dados do formulário para a API
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarPlanosAsync();
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o novo membro para a API
                var resposta = await client.PostAsJsonAsync(
                    "api/Membros",
                    Membro
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
                    $"Não foi possível criar o membro: {ex.Message}";
            }

            // Volta a carregar os planos se acontecer algum erro
            await CarregarPlanosAsync();

            return Page();
        }

        // Vai buscar os planos ativos à API
        private async Task CarregarPlanosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var planos = await client.GetFromJsonAsync<List<PlanoOptionModel>>(
                    "api/Planos"
                );

                if (planos != null)
                {
                    Planos = planos
                        .Where(p => p.Ativo)
                        .OrderBy(p => p.Nome)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = $"{p.Nome} - {p.Preco:0.00} €"
                        })
                        .ToList();
                }
            }
            catch
            {
                MensagemErro =
                    "Não foi possível carregar os planos disponíveis.";
            }
        }
    }

    // Modelo usado para receber os dados do formulário
    public class MembroInputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
        public string? Telefone { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        public bool Ativo { get; set; } = true;

        [Display(Name = "Plano")]
        [Range(1, int.MaxValue, ErrorMessage = "Escolha um plano.")]
        public int PlanoId { get; set; }
    }

    // Modelo usado para preencher a lista dos planos
    public class PlanoOptionModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}