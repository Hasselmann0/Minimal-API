using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Enuns;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infra.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => 
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login efetuado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }     
}).WithTags("Administradores");

app.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagem = new List<string>()
    };
    if (string.IsNullOrEmpty(administradorDTO.Email))
    {
        validacao.Mensagem.Add("Email é obrigatório!");
    }
    if (string.IsNullOrEmpty(administradorDTO.Senha))
    {
        validacao.Mensagem.Add("Senha é obrigatória!");
    }
    if (administradorDTO.Perfil == null)
    {
        validacao.Mensagem.Add("Perfil é obrigatório!");
    }
        var administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };
        administradorServico.Incluir(administrador); 
        return Results.Created($"/Administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });  
    }).WithTags("Administradores");

    app.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
    {
        var adms = new List<AdministradorModelView>();
        var administradores = administradorServico.Todos(pagina);
        foreach(var adm in administradores)
        {
            adms.Add(new AdministradorModelView
            {
                Id = adm.Id,
                Email = adm.Email,
                Perfil = adm.Perfil
            });
        }
        return Results.Ok(adms); 
    }).WithTags("Administradores");

    app.MapGet("/Administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => 
    {
        var administrador = administradorServico.BuscarPorId(id);
        if (administrador == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }).WithTags("Administradores");
#endregion

#region Veiculos
app.MapPost("/Veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => 
{
    var Mensagem =  new ErrosDeValidacao();
    if (veiculoDTO.Nome == null || veiculoDTO.Marca == null || veiculoDTO.Ano == 0)
    {
        Mensagem.Mensagem = new List<string> { "Nome, Marca e Ano são obrigatórios!" };
        return Results.BadRequest(Mensagem);
    }
    
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/Veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/Veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.Todos(pagina);

    return Results.Ok(veiculo); 
}).WithTags("Veiculos");

app.MapGet("/Veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo); 
}).WithTags("Veiculos");

app.MapPut("/Veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo); 
}).WithTags("Veiculos");

app.MapDelete("/Veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
