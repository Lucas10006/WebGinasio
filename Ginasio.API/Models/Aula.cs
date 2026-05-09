namespace Ginasio.API.Models
{
    public class Aula
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Instrutor { get; set; }

        // Uma aula tem muitos membros inscritos 
        public ICollection<Membro> Membros { get; set; }
    }
}
