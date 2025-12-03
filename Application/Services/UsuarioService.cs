using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(MapToReadDto);
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        return usuario == null ? null : MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        // Normalizar email para lowercase
        var emailNormalizado = dto.Email.ToLowerInvariant();

        // Verificar se email já existe
        if (await _repository.EmailExistsAsync(emailNormalizado, ct))
        {
            throw new InvalidOperationException("Email já cadastrado");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = emailNormalizado,
            Senha = dto.Senha, // Em produção, fazer hash da senha
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado");
        }

        // Normalizar email para lowercase
        var emailNormalizado = dto.Email.ToLowerInvariant();

        // Verificar se o email já existe para outro usuário
        var usuarioComEmail = await _repository.GetByEmailAsync(emailNormalizado, ct);
        if (usuarioComEmail != null && usuarioComEmail.Id != id)
        {
            throw new InvalidOperationException("Email já cadastrado para outro usuário");
        }

        usuario.Nome = dto.Nome;
        usuario.Email = emailNormalizado;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null)
        {
            return false;
        }

        // Soft delete: marcar como inativo
        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        var emailNormalizado = email.ToLowerInvariant();
        return await _repository.EmailExistsAsync(emailNormalizado, ct);
    }

    private static UsuarioReadDto MapToReadDto(Usuario usuario)
    {
        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }
}
