using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Infra.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configuracaoAPPSettings;
        
        public DbContexto(IConfiguration configuracaoAPPSettings)
        {
            _configuracaoAPPSettings = configuracaoAPPSettings;
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador {
                    Id = 1,
                    Email = "administrador@teste.com", 
                    Senha = "123456",
                    Perfil = "Adm"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var stringConexao = _configuracaoAPPSettings.GetConnectionString("ConexaoPadrao")?.ToString();
                if(!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseSqlServer(stringConexao);
                }
            }
        }
        
    }
}