using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infra.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();


app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => 
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login efetuado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }     
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

