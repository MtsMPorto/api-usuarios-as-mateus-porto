using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using APIUsuarios.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configurar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===== ENDPOINTS =====

// GET /usuarios - Lista todos os usuários
app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuarios = await service.ListarAsync(ct);
        return Results.Ok(usuarios);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro interno do servidor"
        );
    }
})
.WithName("ListarUsuarios")
.WithTags("Usuarios")
.Produces<IEnumerable<UsuarioReadDto>>(200)
.Produces(500);

// GET /usuarios/{id} - Busca usuário por ID
app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuario = await service.ObterAsync(id, ct);
        return usuario == null
            ? Results.NotFound(new { message = "Usuário não encontrado" })
            : Results.Ok(usuario);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro interno do servidor"
        );
    }
})
.WithName("ObterUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(200)
.Produces(404)
.Produces(500);

// POST /usuarios - Cria novo usuário
app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IUsuarioService service,
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    try
    {
        // Validar DTO
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                errors = validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }

        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Email já cadastrado"))
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro interno do servidor"
        );
    }
})
.WithName("CriarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(201)
.Produces(400)
.Produces(409)
.Produces(500);

// PUT /usuarios/{id} - Atualiza usuário completo
app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    CancellationToken ct) =>
{
    try
    {
        // Validar DTO
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                errors = validationResult.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }

        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Email já cadastrado"))
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro interno do servidor"
        );
    }
})
.WithName("AtualizarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(200)
.Produces(400)
.Produces(404)
.Produces(409)
.Produces(500);

// DELETE /usuarios/{id} - Remove usuário (soft delete)
app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var removido = await service.RemoverAsync(id, ct);
        return removido
            ? Results.NoContent()
            : Results.NotFound(new { message = "Usuário não encontrado" });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Erro interno do servidor"
        );
    }
})
.WithName("RemoverUsuario")
.WithTags("Usuarios")
.Produces(204)
.Produces(404)
.Produces(500);

app.Run();
