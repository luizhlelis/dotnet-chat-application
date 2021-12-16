using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ChatApi.Infrastructure;
using ChatApi.Application;
using ChatApi.Domain;

namespace ChatApi.Test.Support
{
    public class DatabaseFixture : IDisposable
    {
        public HttpClient Client;
        public ChatContext DbContext;
        private readonly TestingWebApplicationFactory<Startup> _factory;
        private readonly IDbContextTransaction _transaction;
        private readonly IServiceScope _testServiceScope;

        public DatabaseFixture(Action<IServiceCollection> setupAction)
        {
            setupAction ??= TestServiceCollections.DefaultTestServices;

            // constructs the testing server with the HostBuilder configuration
            _factory = new TestingWebApplicationFactory<Startup>(setupAction);
            Client = _factory.CreateClient();

            // Begin test service scope
            _testServiceScope = _factory.Services.CreateScope();

            // Open a transaction to not commit tests changes to db
            DbContext = _testServiceScope.ServiceProvider.GetRequiredService<ChatContext>();
            _transaction = DbContext.Database.BeginTransaction();

            var testUserPassword = "1StrongPassword*";

            DbContext.Users.Add(new User("test-user", testUserPassword.GetHashSha256()));
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            Client?.Dispose();
            _testServiceScope.Dispose();

            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
