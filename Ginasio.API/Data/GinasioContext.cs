using Ginasio.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ginasio.API.Data
{
    // Esta classe faz a ligação entre os modelos e a base de dados
    public class GinasioContext : DbContext
    {
        public GinasioContext(DbContextOptions<GinasioContext> options)
            : base(options)
        {
        }

        // Tabelas da base de dados
        public DbSet<Membro> Membros { get; set; }
        public DbSet<Plano> Planos { get; set; }
        public DbSet<Aula> Aulas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Um plano pode ter vários membros
            // Cada membro só pode ter um plano
            modelBuilder.Entity<Membro>()
                .HasOne(m => m.Plano)
                .WithMany(p => p.Membros)
                .HasForeignKey(m => m.PlanoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Um membro pode estar inscrito em várias aulas
            // Uma aula pode ter vários membros
            modelBuilder.Entity<Membro>()
                .HasMany(m => m.Aulas)
                .WithMany(a => a.Membros);

            // O email de cada membro deve ser único
            modelBuilder.Entity<Membro>()
                .HasIndex(m => m.Email)
                .IsUnique();
        }
    }
}