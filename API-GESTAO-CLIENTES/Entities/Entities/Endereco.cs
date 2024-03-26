using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Endereco : Notifies
    {
        public int Id { get; set; } 
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}
