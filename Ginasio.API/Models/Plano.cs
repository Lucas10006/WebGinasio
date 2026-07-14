using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Models
{
    // Esta classe representa um plano disponível no ginásio
    public class Plano
    {
        public int Id { get; set; }

        // O nome do plano é obrigatório
        [Required(ErrorMessage = "O nome do plano é obrigatório.")]
        [StringLength(
            60,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 60 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        // A descrição não é obrigatória
        [StringLength(
            300,
            ErrorMessage = "A descrição não pode ter mais de 300 caracteres."
        )]
        public string? Descricao { get; set; }

        // O preço tem de ser maior do que zero
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(
            0.01,
            1000,
            ErrorMessage = "O preço deve estar entre 0,01€ e 1000€."
        )]
        public decimal Preco { get; set; }

        // Indica se o plano ainda está disponível
        public bool Ativo { get; set; } = true;

        // Um plano pode estar associado a vários membros
        public ICollection<Membro> Membros { get; set; }
            = new List<Membro>();
    }
}