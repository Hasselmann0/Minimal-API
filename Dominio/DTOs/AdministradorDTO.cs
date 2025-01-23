using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_API.Dominio.Enuns;

namespace Minimal_API.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfil? Perfil { get; set; } = default!;
    }
}