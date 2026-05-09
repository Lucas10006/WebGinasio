namespace Ginasio.API.Models
{
    public class Membro
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        //Muitos para um. O membro vai pertencer a um plano.
        public int PlanoId { get; set; }
        public Plano Plano { get; set; }

        // Muitos para muitos. O membro pode pertencer a várias aulas.
        public ICollection<Aula> Aulas { get; set; }
    }
}
