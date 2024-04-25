
using ETicket.Db.Dal;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Db.Web.Host
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Startup>()
                .Build();

            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
           AddLocalDbContext(services);
        }

        private void AddLocalDbContext(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("ETicketDb");
            services.AddDbContext<ETicketDbContext>(options =>
                options.UseSqlServer(connection));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }
    }
}
