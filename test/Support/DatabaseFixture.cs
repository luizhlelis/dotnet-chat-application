using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ChatApi.Infrastructure;

namespace ChatApi.Test.Support
{
    public class DatabaseFixture : IDisposable
    {
        public HttpClient Client;
        public ChatContext DbContext;
        private readonly TestingWebApplicationFactory<Startup> _factory;
        private readonly IDbContextTransaction _transaction;
        private readonly IServiceScope _testServiceScope;

        public DatabaseFixture()
        {
            // constructs the testing server with the HostBuilder configuration
            _factory = new TestingWebApplicationFactory<Startup>();
            Client = _factory.CreateClient();

            // Begin test service scope
            _testServiceScope = _factory.Services.CreateScope();

            // Open a transaction to not commit tests changes to db
            DbContext = _testServiceScope.ServiceProvider.GetRequiredService<ChatContext>();
            _transaction = DbContext.Database.BeginTransaction();

            DbContext.Users.Add(new User("test-user", "1StrongPassword*"));
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
