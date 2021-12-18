using FluentValidation;

namespace BotApi.Domain.DTOs
{
    public class CommandDto
    {
        public string Name { get; init; }
        public string Value { get; init; }

        public CommandDto(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class CommandDtoValidator : AbstractValidator<CommandDto>
    {
        public CommandDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(20);
        }
    }
}
