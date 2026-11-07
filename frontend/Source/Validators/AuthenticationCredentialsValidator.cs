namespace Apta.WebUI.Validators;

public sealed class AuthenticationCredentialsValidator : AbstractValidator<AuthenticationCredentialsScheme>
{
    public AuthenticationCredentialsValidator()
    {
        RuleFor(credentials => credentials.Email)
            .NotEmpty()
            .WithMessage("Informe o seu email.")
            .EmailAddress()
            .WithMessage("Informe um email válido.");

        RuleFor(credentials => credentials.Password)
            .NotEmpty()
            .WithMessage("Informe a sua senha.")
            .MinimumLength(6)
            .WithMessage("A senha deve conter ao menos 6 caracteres.");
    }
}
