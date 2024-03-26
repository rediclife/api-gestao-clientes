using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    //[Table("Cliente")]
    public class Cliente : Notifies
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Endereco? Endereco { get; set; }
        public virtual List<Contato> Contatos { get; set; }
        
    }
}
