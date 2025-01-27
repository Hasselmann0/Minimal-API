using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Enuns;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infra.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "1234567890123456";

builder.Services.AddAuthentication(option => 
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(Options =>
{
    Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Autorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });
    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<DbContexto>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt (Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken
    (
        claims : claims,
        expires : DateTime.Now.AddHours(1),
        signingCredentials : credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
app.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => 
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }     
}).AllowAnonymous().WithTags("Administradores");

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
    }).RequireAuthorization().WithTags("Administradores");

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
    }).RequireAuthorization().WithTags("Administradores");

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
    }).RequireAuthorization().WithTags("Administradores");
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
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/Veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.Todos(pagina);

    return Results.Ok(veiculo); 
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/Veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo); 
}).RequireAuthorization().WithTags("Veiculos");

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
}).RequireAuthorization().WithTags("Veiculos");

app.MapDelete("/Veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => 
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).RequireAuthorization().WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
