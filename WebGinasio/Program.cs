using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte às Razor Pages
builder.Services.AddRazorPages();

// Configura a autenticação através de cookies
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página para onde o utilizador é enviado
        // quando não tem sessão iniciada
        options.LoginPath = "/Login/Login";

        // Página apresentada quando o utilizador
        // não tem permissões suficientes
        options.AccessDeniedPath = "/AccessDenied";

        // Tempo máximo de validade da sessão
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // Renova o tempo da sessão enquanto
        // o utilizador continua a usar o site
        options.SlidingExpiration = true;
    });

// Adiciona o serviço de autorização
builder.Services.AddAuthorization();

// Regista o HttpClient usado para comunicar com a API
builder.Services.AddHttpClient("API", client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiUrl))
    {
        throw new InvalidOperationException(
            "O endereço da API não está configurado no appsettings.json."
        );
    }

    // Define o endereço principal da API
    client.BaseAddress = new Uri(apiUrl);

    // Usa HTTP/1.1 para evitar problemas na ligação local
    client.DefaultRequestVersion = HttpVersion.Version11;
    client.DefaultVersionPolicy =
        HttpVersionPolicy.RequestVersionOrLower;
});

var app = builder.Build();

// Configuração usada em ambiente de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Durante o desenvolvimento estamos a usar HTTP
// app.UseHttpsRedirection();

// Permite carregar CSS, JavaScript e imagens
app.UseStaticFiles();

app.UseRouting();

// Verifica primeiro quem é o utilizador
app.UseAuthentication();

// Depois verifica as permissões do utilizador
app.UseAuthorization();

// Ativa as Razor Pages
app.MapRazorPages();

app.Run();