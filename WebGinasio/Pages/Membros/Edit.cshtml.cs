using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Membros
{
    // Esta página permite editar os dados de um membro
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Guarda os dados do membro no formulário
        [BindProperty]
        public MembroEditModel Membro { get; set; } = new();

        // Lista usada para escolher o plano do membro
        public List<SelectListItem> Planos { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados do membro e os planos
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Vai buscar o membro através do seu ID
                var membro = await client.GetFromJsonAsync<MembroEditModel>(
                    $"api/Membros/{id}"
                );

                if (membro == null)
                {
                    return NotFound();
                }

                Membro = membro;

                // Carrega os planos para a caixa de seleção
                await CarregarPlanosAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar o membro: {ex.Message}";
            }

            return Page();
        }

        // Envia as alterações para a API
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

                // Envia os dados atualizados do membro
                var resposta = await client.PutAsJsonAsync(
                    $"api/Membros/{Membro.Id}",
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
                    $"Não foi possível editar o membro: {ex.Message}";
            }

            // É necessário carregar novamente os planos
            await CarregarPlanosAsync();

            return Page();
        }

        // Vai buscar os planos disponíveis à API
        private async Task CarregarPlanosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var planos = await client.GetFromJsonAsync<List<PlanoEditOptionModel>>(
                    "api/Planos"
                );

                if (planos != null)
                {
                    Planos = planos
                        .Where(p => p.Ativo || p.Id == Membro.PlanoId)
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

    // Modelo usado para editar os dados do membro
    public class MembroEditModel
    {
        public int Id { get; set; }

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

        public bool Ativo { get; set; }

        [Display(Name = "Plano")]
        [Range(1, int.MaxValue, ErrorMessage = "Escolha um plano.")]
        public int PlanoId { get; set; }
    }

    // Modelo usado para preencher a lista dos planos
    public class PlanoEditOptionModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}