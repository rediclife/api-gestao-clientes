using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTOs
{
    public class EnderecoDTO
    {
        public int Id { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Cidade { get; set; }
        public string? Bairro { get; set; }  
        public int? Numero { get; set; }
        public string? Complemento { get; set; }
        public int ClienteId { get; set; }
    }
}
