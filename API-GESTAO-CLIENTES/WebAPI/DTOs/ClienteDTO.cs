using Entities.Entities;

namespace WebAPI.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public DateTime? DataCadastro { get; set; }
        public EnderecoDTO? Endereco { get; set; }
        public List<ContatoDTO>? Contatos { get; set; }
    }
}
