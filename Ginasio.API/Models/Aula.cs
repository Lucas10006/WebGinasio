using System.ComponentModel.DataAnnotations;

namespace Ginasio.API.Models
{
    // Esta classe representa uma aula do ginásio
    public class Aula
    {
        public int Id { get; set; }

        // O nome da aula é obrigatório
        [Required(ErrorMessage = "O nome da aula é obrigatório.")]
        [StringLength(
            80,
            MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 80 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

        // Nome do instrutor responsável pela aula
        [Required(ErrorMessage = "O nome do instrutor é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "O nome do instrutor deve ter entre 3 e 100 caracteres."
        )]
        public string Instrutor { get; set; } = string.Empty;

        // Data e hora em que a aula vai acontecer
        [Required(ErrorMessage = "A data e hora são obrigatórias.")]
        [Display(Name = "Data e Hora")]
        public DateTime DataHora { get; set; }

        // Duração da aula em minutos
        [Display(Name = "Duração")]
        [Range(
            15,
            240,
            ErrorMessage = "A duração deve estar entre 15 e 240 minutos."
        )]
        public int DuracaoMinutos { get; set; } = 60;

        // Número máximo de membros que podem participar
        [Display(Name = "Lotação Máxima")]
        [Range(
            1,
            100,
            ErrorMessage = "A lotação deve estar entre 1 e 100 pessoas."
        )]
        public int LotacaoMaxima { get; set; } = 20;

        // Indica se a aula está disponível
        public bool Ativa { get; set; } = true;

        // Lista dos membros inscritos na aula
        public ICollection<Membro> Membros { get; set; }
            = new List<Membro>();
    }
}