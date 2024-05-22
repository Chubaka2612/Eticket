using ETicket.Api;
using ETicket.Db.Dal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;

namespace AutomotivePlatform.PLI.WebPortal.API.IntegrationTests
{
    public class InMemoryWebFactory : WebApplicationFactory<Startup>
    {
        private readonly string _dbName;

        public InMemoryWebFactory(string dbName) => _dbName = dbName;

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.ConfigureTestServices(services =>
            {
                var dbContextOptions =
                    services.Single(x => x.ServiceType == typeof(DbContextOptions<ETicketDbContext>));

                services.Remove(dbContextOptions);

                services.AddDbContext<ETicketDbContext>(optionsBuilder =>
                {
                    optionsBuilder.UseInMemoryDatabase(_dbName)
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
            });

        public HttpClient CreateNoRedirectClient() => CreateClient(new() { AllowAutoRedirect = false });
    }
}