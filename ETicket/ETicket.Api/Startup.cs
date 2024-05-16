
using ETicket.Bll.Services;
using ETicket.Bll.Services.Cart;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ETicket.Api
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
            var connection = Configuration.GetConnectionString("ETicketDb");
            services.AddDbContext<ETicketDbContext>(options =>
                options.UseSqlServer(connection));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<IUnitOfWork, ETicketUnitOfWork>();
            services.AddScoped<IVenueService, VenueService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddSingleton<ICartStorage, CartStorage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
}
