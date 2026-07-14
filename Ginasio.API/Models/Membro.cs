using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Models
{
    // Esta classe representa um membro do ginásio
    public class Membro
    {
        public int Id { get; set; }

        // O nome é obrigatório
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        // O email é obrigatório
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        // O telefone é opcional
        [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
        public string? Telefone { get; set; }

        // Data de nascimento do membro
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        // Data em que o membro foi registado
        [Display(Name = "Data de Inscrição")]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        // Indica se o membro está ativo
        public bool Ativo { get; set; } = true;

        // Chave estrangeira para o plano
        [Display(Name = "Plano")]
        public int PlanoId { get; set; }

        // Plano associado ao membro
        public Plano? Plano { get; set; }

        // Lista das aulas em que o membro está inscrito
        public ICollection<Aula> Aulas { get; set; }
            = new List<Aula>();
    }
}