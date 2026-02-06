using FluentValidation;
using TherapuHubAPI.DTOs.Requests.Users;

namespace TherapuHubAPI.Validators.Users;

public class CreateUsuarioRequestDtoValidator : AbstractValidator<CreateUsuarioRequestDto>
{
    public CreateUsuarioRequestDtoValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");

        RuleFor(x => x.UserTypeId)
            .GreaterThan(0).WithMessage("User type is required");
    }
}
