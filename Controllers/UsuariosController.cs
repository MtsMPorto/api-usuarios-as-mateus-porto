using Microsoft.AspNetCore.Mvc;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using FluentValidation;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IValidator<UsuarioCreateDto> _createValidator;
    private readonly IValidator<UsuarioUpdateDto> _updateValidator;

    public UsuariosController(
        IUsuarioService usuarioService,
        IValidator<UsuarioCreateDto> createValidator,
        IValidator<UsuarioUpdateDto> updateValidator)
    {
        _usuarioService = usuarioService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Lista todos os usuários ativos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UsuarioReadDto>>> GetAll(CancellationToken ct)
    {
        var usuarios = await _usuarioService.ListarAsync(ct);
        return Ok(usuarios);
    }

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioReadDto>> GetById(int id, CancellationToken ct)
    {
        var usuario = await _usuarioService.ObterAsync(id, ct);

        if (usuario == null)
            return NotFound(new { message = $"Usuário com ID {id} não encontrado." });

        return Ok(usuario);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UsuarioReadDto>> Create([FromBody] UsuarioCreateDto dto, CancellationToken ct)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var usuario = await _usuarioService.CriarAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UsuarioReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioReadDto>> Update(int id, [FromBody] UsuarioUpdateDto dto, CancellationToken ct)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var usuario = await _usuarioService.AtualizarAsync(id, dto, ct);

            return Ok(usuario);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Usuário com ID {id} não encontrado." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var removed = await _usuarioService.RemoverAsync(id, ct);

        if (!removed)
            return NotFound(new { message = $"Usuário com ID {id} não encontrado." });

        return NoContent();
    }
}
