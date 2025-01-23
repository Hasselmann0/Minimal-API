using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimal_API.Dominio.DTOs
{
    public record VeiculoDTO
    {
        public String Nome { get; set; } = default!;
        public String Marca { get; set; } = default!;
        public int Ano { get; set; } = default!;
    }
}