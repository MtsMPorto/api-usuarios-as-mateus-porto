using APIUsuarios.Application.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace APIUsuarios.Application.Validators;

public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
{
    public UsuarioCreateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ser válido");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .Must(BeAtLeast18YearsOld).WithMessage("Usuário deve ter pelo menos 18 anos");

        RuleFor(x => x.Telefone)
            .Must(BeValidPhoneFormat).When(x => !string.IsNullOrEmpty(x.Telefone))
            .WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX");
    }

    private bool BeAtLeast18YearsOld(DateTime dataNascimento)
    {
        var today = DateTime.Today;
        var age = today.Year - dataNascimento.Year;
        if (dataNascimento.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }

    private bool BeValidPhoneFormat(string? telefone)
    {
        if (string.IsNullOrEmpty(telefone)) return true;
        var phoneRegex = new Regex(@"^$$\d{2}$$\s?\d{5}-\d{4}$");
        return phoneRegex.IsMatch(telefone);
    }
}
