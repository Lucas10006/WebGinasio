using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Claims;

namespace WebGinasio.Pages.Login
{
    // Modelo da página de login
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe o HttpClient configurado no Program.cs
        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Email introduzido pelo utilizador
        [BindProperty]
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        // Palavra-passe introduzida pelo utilizador
        [BindProperty]
        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Mensagem apresentada quando o login falha
        public string? MensagemErro { get; set; }

        // Executado quando a página é aberta
        public IActionResult OnGet()
        {
            // Se o utilizador já tiver sessão iniciada,
            // volta para a página inicial
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        // Executado quando o utilizador carrega em Entrar
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("API");

            // Dados enviados para a API
            var pedido = new LoginPedido
            {
                Email = Email,
                Password = Password
            };

            HttpResponseMessage response;

            try
            {
                // Envia o pedido para o endpoint criado no AuthController
                response = await client.PostAsJsonAsync(
                    "api/Auth/login",
                    pedido
                );
            }
            catch (HttpRequestException)
            {
                MensagemErro = "Não foi possível comunicar com a API.";
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                MensagemErro = response.StatusCode ==
                               System.Net.HttpStatusCode.Unauthorized
                    ? "Email ou palavra-passe incorretos."
                    : "Não foi possível iniciar sessão.";

                return Page();
            }

            // Lê os dados devolvidos pela API
            var resposta = await response.Content
                .ReadFromJsonAsync<LoginResposta>();

            if (resposta == null)
            {
                MensagemErro = "Ocorreu um erro ao iniciar sessão.";
                return Page();
            }

            // Informações guardadas no cookie
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    resposta.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    resposta.Nome
                ),

                new Claim(
                    ClaimTypes.Email,
                    resposta.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    resposta.TipoUtilizador
                )
            };

            // Se for uma conta de membro, guarda também o ID do membro
            if (resposta.MembroId.HasValue)
            {
                claims.Add(
                    new Claim(
                        "MembroId",
                        resposta.MembroId.Value.ToString()
                    )
                );
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            // Cria o cookie de autenticação
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false
                }
            );

            return RedirectToPage("/Index");
        }
    }

    // Dados enviados para a API
    public class LoginPedido
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    // Dados recebidos da API
    public class LoginResposta
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TipoUtilizador { get; set; } = string.Empty;

        public int? MembroId { get; set; }
    }
}