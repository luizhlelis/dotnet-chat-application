using FluentValidation;

namespace ChatApi.Domain.DTOs
{
    public class CredentialsDto
    {
        public string Username { get; init; }
        public string Password { get; init; }
    }

    public class CredentialsValidator : AbstractValidator<CredentialsDto>
    {
        public CredentialsValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
