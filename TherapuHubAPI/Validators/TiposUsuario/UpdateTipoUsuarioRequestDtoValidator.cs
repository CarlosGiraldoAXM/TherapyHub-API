using FluentValidation;
using TherapuHubAPI.DTOs.Requests.UserTypes;

namespace TherapuHubAPI.Validators.UserTypes;

public class UpdateTipoUsuarioRequestDtoValidator : AbstractValidator<UpdateTipoUsuarioRequestDto>
{
    public UpdateTipoUsuarioRequestDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
