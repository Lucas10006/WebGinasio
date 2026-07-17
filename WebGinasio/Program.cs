using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte às Razor Pages
builder.Services.AddRazorPages();

// Configura a autenticação através de cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página para onde o utilizador é enviado caso não tenha sessão iniciada
        options.LoginPath = "/Login";
        // Página apresentada quando o utilizador não tem permissões suficientes
        options.AccessDeniedPath = "/AccessDenied";
        // Tempo máximo de validade da sessão
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Regista o HttpClient usado para comunicar com a API
builder.Services.AddHttpClient("API", client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"];

    // Define o endereço principal da API
    client.BaseAddress = new Uri(apiUrl!);

    // Usa HTTP/1.1 para evitar problemas na ligação local
    client.DefaultRequestVersion = HttpVersion.Version11;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
});

var app = builder.Build();

// Configuração usada em ambiente de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redireciona os pedidos para HTTPS
app.UseHttpsRedirection();

// Permite carregar CSS, JavaScript e imagens
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Ativa as Razor Pages
app.MapRazorPages();

app.Run();