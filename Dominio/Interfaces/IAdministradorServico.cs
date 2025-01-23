using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina);
        Administrador? BuscarPorId(int id);
    }
}