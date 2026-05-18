using System.Collections.Generic;

namespace Ginasio.API.Models
{
    // Esta classe representa os planos de subscrição do ginásio
    public class Plano
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public decimal Preco { get; set; }

        // Relacionamento Muitos-para-Um: Um plano tem muitos membros 
        // Ao inicializar com 'new List<Membro>()', dizemos ao .NET que o campo não é obrigatório no POST
        public ICollection<Membro> Membros { get; set; } = new List<Membro>();
    }
}