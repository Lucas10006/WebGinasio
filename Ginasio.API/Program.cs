using Ginasio.API.Data;
using Ginasio.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura a ligação à base de dados
builder.Services.AddDbContext<GinasioContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Adiciona os Controllers da API
// O IgnoreCycles evita erros nas relações entre os objetos
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Serviço usado para proteger as palavras-passe
builder.Services.AddScoped<
    IPasswordHasher<Utilizador>,
    PasswordHasher<Utilizador>
>();

// Adiciona o Swagger para testar a API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa o Swagger durante o desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Durante o desenvolvimento estamos a usar HTTP entre os projetos
// app.UseHttpsRedirection();

app.UseAuthorization();

// Ativa os Controllers da API
app.MapControllers();

app.Run();