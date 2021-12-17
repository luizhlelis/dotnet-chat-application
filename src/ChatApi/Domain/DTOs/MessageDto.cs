using System;
using FluentValidation;

namespace ChatApi.Domain.DTOs
{
    public class MessageDto
    {
        public string Content { get; init; }
        public Guid ChatRoomId { get; init; }

        public MessageDto(string content, Guid chatRoomId)
        {
            Content = content;
            ChatRoomId = chatRoomId;
        }
    }

    public class MessageDtoValidator : AbstractValidator<MessageDto>
    {
        public MessageDtoValidator()
        {
            RuleFor(x => x.Content).NotNull().NotEmpty().MaximumLength(300);
            RuleFor(x => x.ChatRoomId).NotNull().NotEmpty();
        }
    }
}
