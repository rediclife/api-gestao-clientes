﻿namespace WebAPI.DTOs
{
    public class ContatoDTO
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Texto { get; set; }
        public int? ClienteId { get; set; }
    }
}
