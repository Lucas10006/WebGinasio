var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte às Razor Pages
builder.Services.AddRazorPages();

// Regista um HttpClient para comunicar com a API
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!
    );
});

var app = builder.Build();

// Configuração para ambiente de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redireciona automaticamente para HTTPS
app.UseHttpsRedirection();

// Permite usar ficheiros da pasta wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapeia as Razor Pages
app.MapRazorPages();

app.Run();