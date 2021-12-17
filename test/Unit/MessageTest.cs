using System;
using ChatApi.Domain.Entities;
using Xunit;
using Moq;
using ChatApi.Domain.Notifications;
using FluentAssertions;
using Flurl.Http.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ChatApi.Test.Unit
{
    public class MessageTest
    {
        private readonly INotificationContext _mockedNotificationContext;
        private readonly IConfiguration _mockedConfiguration;

        public MessageTest()
        {
            var mockedNotificationContext = new Mock<INotificationContext>();
            mockedNotificationContext
                .Setup(x => x.AddNotification(It.IsAny<int>(), It.IsAny<string>()));

            _mockedNotificationContext = mockedNotificationContext.Object;

            var inMemorySettings = new Dictionary<string, string> {
                {"Http:BotApi", "http://testing-host/v1/command"}
            };

            _mockedConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact(DisplayName = "Should return true when command validation")]
        public void ShouldReturnTrueWhenCommandValidation()
        {
            // Arrange
            var message = new Message("/stock=stock_code", string.Empty, Guid.Empty);

            // Act / Assert
            message.GetCommand().Should().BeEquivalentTo(new Tuple<string, string>("stock", "stock_code"));
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
            message.NotificationContext = _mockedNotificationContext;

            // Act / Assert
            message.GetCommand().Should().BeEquivalentTo(new Tuple<string, string>("", ""));
        }

        [Fact(DisplayName = "Should call bot api when command is sent")]
        public async Task ShouldCallBotApiWhenCommandIsSent()
        {
            using var httpTest = new HttpTest();

            // Arrange
            var message = new Message("/stock=stock_code", string.Empty, Guid.Empty);
            message.Configuration = _mockedConfiguration;
            httpTest.RespondWith("OK", 200);

            // Act
            await message.Send();

            // Assert
            httpTest
                .ShouldHaveCalled(_mockedConfiguration["Http:BotApi"])
                .WithVerb(HttpMethod.Post)
                .WithContentType("application/json");
        }
    }
}
