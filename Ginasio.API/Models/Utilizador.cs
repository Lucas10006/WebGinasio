using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Models
{
    // Esta classe representa um utilizador que pode entrar na aplicação
    public class Utilizador
    {
        public int Id { get; set; }

        // Nome apresentado dentro da aplicação
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        // O email vai ser usado para iniciar sessão
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        // Guarda a palavra-passe de forma protegida
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Define as permissões do utilizador
        [Required]
        public string TipoUtilizador { get; set; } = "Membro";

        // Indica se o utilizador pode entrar na aplicação
        public bool Ativo { get; set; } = true;

        // Ligação opcional aos dados de um membro
        public int? MembroId { get; set; }

        // Membro associado à conta
        public Membro? Membro { get; set; }
    }
}