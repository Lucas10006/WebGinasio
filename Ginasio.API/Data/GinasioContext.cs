using Ginasio.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ginasio.API.Data
{
    public class GinasioContext : DbContext
    {
        public GinasioContext(DbContextOptions<GinasioContext> options) : base(options) { }

        public DbSet<Membro> Membros { get; set; }
        public DbSet<Plano> Planos { get; set; }
        public DbSet<Aula> Aulas { get; set; }

        // Configuração adicional para os relacionamentos obrigatórios 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a relação Muitos-para-Muitos entre Membro e Aula
            modelBuilder.Entity<Membro>()
                .HasMany(m => m.Aulas)
                .WithMany(a => a.Membros);
        }
    }
}