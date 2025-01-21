using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Infra.Db;

namespace Minimal_API.Dominio.Servicos
{
    public class AdminstradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;

        public AdminstradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
                return adm; 
        }
    }   
}