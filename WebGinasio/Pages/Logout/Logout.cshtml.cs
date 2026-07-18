using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebGinasio.Pages
{
    // Página responsável por terminar a sessão do utilizador
    public class LogoutModel : PageModel
    {
        // Executado quando o utilizador faz logout
        public async Task<IActionResult> OnGetAsync()
        {
            // Remove o cookie de autenticação
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Depois do logout volta para a página de login
            return RedirectToPage("/Login/Login");
        }
    }
}