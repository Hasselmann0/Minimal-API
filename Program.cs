using Minimal_API.Dominio.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => 
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("Login efetuado com sucesso!");
    }
    else
    {
        return Results.BadRequest("Usuário ou senha inválidos!");
    }     
});

app.Run();

