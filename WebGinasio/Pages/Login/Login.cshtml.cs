using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace WebGinasio.Pages
{
    // Modelo da página de Login
    public class LoginModel : PageModel
    {
        // Permite comunicar com a API
        private readonly IHttpClientFactory _httpClientFactory;

        // Recebe a fábrica de HttpClient através da injeção de dependências
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
        public string Password { get; set; } = string.Empty;

        // Mensagem apresentada caso ocorra algum erro
        public string MensagemErro { get; set; } = string.Empty;

        // Executado quando a página é aberta
        public IActionResult OnGet()
        {
            // Se já estiver autenticado, volta para a página inicial
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        // Executado quando o utilizador carrega no botão "Entrar"
        public async Task<IActionResult> OnPostAsync()
        {
            // Verifica se os dados introduzidos são válidos
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Obtém o HttpClient configurado no Program.cs
            var client = _httpClientFactory.CreateClient("API");

            // Cria o pedido que será enviado para a API
            var pedido = new LoginPedido
            {
                Email = Email,
                Password = Password
            };

            // Converte o pedido para JSON
            var json = JsonSerializer.Serialize(pedido);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            // Envia o pedido de login para a API
            var response = await client.PostAsync(
                "api/Utilizadores/Login",
                content
            );

            // Verifica se o login foi bem sucedido
            if (!response.IsSuccessStatusCode)
            {
                MensagemErro = "Email ou palavra-passe incorretos.";
                return Page();
            }

            // Lê os dados devolvidos pela API
            var resposta = await response.Content.ReadFromJsonAsync<LoginResposta>();

            if (resposta == null)
            {
                MensagemErro = "Ocorreu um erro ao iniciar sessão.";
                return Page();
            }

            // Cria os dados de autenticação do utilizador
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, resposta.Id.ToString()),
                new Claim(ClaimTypes.Name, resposta.Nome),
                new Claim(ClaimTypes.Email, resposta.Email),
                new Claim(ClaimTypes.Role, resposta.TipoUtilizador)
            };

            // Cria a identidade do utilizador
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Cria o principal utilizado pela autenticação
            var principal = new ClaimsPrincipal(identity);

            // Guarda o cookie de autenticação
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // Redireciona o utilizador para a página inicial
            return RedirectToPage("/Index");
        }
    }
    // Representa os dados enviados para a API durante o login
    public class LoginPedido
    {
        // Email introduzido pelo utilizador
        public string Email { get; set; } = string.Empty;

        // Palavra-passe introduzida pelo utilizador
        public string Password { get; set; } = string.Empty;
    }

    // Representa os dados devolvidos pela API após um login com sucesso
    public class LoginResposta
    {
        // Identificador do utilizador
        public int Id { get; set; }

        // Nome do utilizador
        public string Nome { get; set; } = string.Empty;

        // Email do utilizador
        public string Email { get; set; } = string.Empty;

        // Tipo de utilizador (Administrador ou Membro)
        public string TipoUtilizador { get; set; } = string.Empty;
    }
}