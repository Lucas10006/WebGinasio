namespace Ginasio.API.Models
{
    public class Aula
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Instrutor { get; set; }

        // Uma aula tem muitos membros inscritos
        // Ao inicializar com 'new List<Membro>()', dizemos ao .NET que o campo não é obrigatório no POST
        public ICollection<Membro> Membros { get; set; } = new List<Membro>();
    }
}
