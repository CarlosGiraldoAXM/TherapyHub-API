using FluentValidation;
using TherapuHubAPI.DTOs.Requests.Auth;

namespace TherapuHubAPI.Validators.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es requerido")
            .EmailAddress().WithMessage("El correo debe tener un formato válido")
            .MaximumLength(255).WithMessage("El correo no puede exceder 255 caracteres");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}
