using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

namespace WebGinasio.Pages.Aulas
{
    // Esta página permite inscrever um membro numa aula
    public class InscreverModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o cliente usado para comunicar com a API
        public InscreverModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ID da aula onde o membro vai ser inscrito
        [BindProperty(SupportsGet = true)]
        public int AulaId { get; set; }

        // ID do membro escolhido no formulário
        [BindProperty]
        public int MembroId { get; set; }

        // Guarda os dados da aula
        public AulaInscricaoModel Aula { get; set; } = new();

        // Lista usada para escolher o membro
        public List<SelectListItem> MembrosDisponiveis { get; set; } = new();

        // Mensagem apresentada caso aconteça algum erro
        public string? MensagemErro { get; set; }

        // Carrega os dados da aula e os membros disponíveis
        public async Task<IActionResult> OnGetAsync(int id)
        {
            AulaId = id;

            var carregou = await CarregarDadosAsync();

            if (!carregou)
            {
                return Page();
            }

            return Page();
        }

        // Inscreve o membro escolhido
        public async Task<IActionResult> OnPostAsync()
        {
            if (MembroId <= 0)
            {
                ModelState.AddModelError(
                    nameof(MembroId),
                    "Escolha um membro."
                );
            }

            if (!ModelState.IsValid)
            {
                await CarregarDadosAsync();
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("API");

                // Envia o pedido de inscrição para a API
                var resposta = await client.PostAsync(
                    $"api/Aulas/{AulaId}/inscrever/{MembroId}",
                    null
                );

                if (resposta.IsSuccessStatusCode)
                {
                    return RedirectToPage(
                        "Details",
                        new { id = AulaId }
                    );
                }

                MensagemErro = await resposta.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível inscrever o membro: {ex.Message}";
            }

            await CarregarDadosAsync();

            return Page();
        }

        // Carrega a aula e os membros que ainda não estão inscritos
        private async Task<bool> CarregarDadosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API");

                var aula = await client.GetFromJsonAsync<AulaInscricaoModel>(
                    $"api/Aulas/{AulaId}"
                );

                if (aula == null)
                {
                    MensagemErro = "Aula não encontrada.";
                    return false;
                }

                Aula = aula;

                var membros = await client.GetFromJsonAsync<List<MembroInscricaoModel>>(
                    "api/Membros"
                );

                if (membros != null)
                {
                    var idsInscritos = Aula.Membros
                        .Select(m => m.Id)
                        .ToList();

                    MembrosDisponiveis = membros
                        .Where(m =>
                            m.Ativo &&
                            !idsInscritos.Contains(m.Id)
                        )
                        .OrderBy(m => m.Nome)
                        .Select(m => new SelectListItem
                        {
                            Value = m.Id.ToString(),
                            Text = $"{m.Nome} - {m.Email}"
                        })
                        .ToList();
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemErro =
                    $"Não foi possível carregar os dados: {ex.Message}";

                return false;
            }
        }
    }

    // Modelo usado para receber os dados da aula
    public class AulaInscricaoModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }

        public int LotacaoMaxima { get; set; }

        public bool Ativa { get; set; }

        public List<MembroInscritoModel> Membros { get; set; } = new();
    }

    // Modelo dos membros que já estão inscritos
    public class MembroInscritoModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }

    // Modelo usado para preencher a lista de membros
    public class MembroInscricaoModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Ativo { get; set; }
    }
}