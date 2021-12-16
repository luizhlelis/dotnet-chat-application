using FluentValidation;

namespace ChatApi.Domain.DTOs
{
    public class UserDto
    {
        public string Username { get; init; }
        public string Password { get; init; }

        public UserDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
