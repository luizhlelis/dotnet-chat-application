using System;
using ChatApi.Domain.Entities;
using Xunit;
using Moq;
using ChatApi.Domain.Notifications;

namespace ChatApi.Test.Unit
{
    public class MessageTest
    {
        private readonly INotificationContext _mockedNotificationContext;

        public MessageTest()
        {
            var mockedNotificationContext = new Mock<INotificationContext>();
            mockedNotificationContext
                .Setup(x => x.AddNotification(It.IsAny<int>(), It.IsAny<string>()));

            _mockedNotificationContext = mockedNotificationContext.Object;
        }

        [Fact(DisplayName = "Should return true when command validation")]
        public void ShouldReturnTrueWhenCommandValidation()
        {
            // Arrange
            var message = new Message("/stock=stock_code", string.Empty, Guid.Empty);

            // Act / Assert
            Assert.True(message.IsCommand());
        }

        [InlineData("/bla=bla")]
        [InlineData("/=bla")]
        [InlineData("bla bla")]
        [InlineData("bla /stock=stock_code bla")]
        [Theory(DisplayName = "Should return false when command validation")]
        public void ShouldReturnFalseWhenCommandValidation(string messageContent)
        {
            // Arrange
            var message = new Message(messageContent, string.Empty, Guid.Empty);
            message.NotifyContext = _mockedNotificationContext;

            // Act / Assert
            Assert.False(message.IsCommand());
        }
    }
}
