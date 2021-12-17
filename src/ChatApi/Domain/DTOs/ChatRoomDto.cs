using FluentValidation;

namespace ChatApi.Domain.DTOs
{
    public class ChatRoomDto
    {
        public string Name { get; init; }

        public ChatRoomDto(string name)
        {
            Name = name;
        }
    }

    public class ChatRoomValidator : AbstractValidator<ChatRoomDto>
    {
        public ChatRoomValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(20);
        }
    }
}
