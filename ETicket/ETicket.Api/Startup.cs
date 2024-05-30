
using ETicket.Bll.Services;
using ETicket.Bll.Services.Cart;
using ETicket.Bll.Services.Cashing;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
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
            // Register EventService and its decorator
            services.AddScoped<IEventService, EventService>();
            services.Decorate<IEventService, CachedEventService>();
            // Register OrderService and its decorator
            services.AddScoped<IOrderService, OrderService>();
            services.Decorate<IOrderService, CachedOrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddSingleton<ICartStorage, CartStorage>();

            services.AddTransient(services =>
            {
                var configuration = services.GetRequiredService<IConfiguration>();
                return configuration.GetRequiredSection("Caching").Get<CacheConfiguration>()
                    ?? throw new InvalidOperationException($"Unable to get the '{typeof(CacheConfiguration)}' from configuration.");
            });

            services.AddSingleton<IDistributedCache, RedisCache>();
            var redisConnection = Configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(redisOption =>
            {
                redisOption.Configuration = redisConnection;
            });

            services.AddSingleton(typeof(ICacheService), typeof(CacheService));

            var cacheExpiration = Configuration.GetRequiredSection("Caching").Get<CacheConfiguration>().SlidingExpirationTimeSpan;
            services.AddOutputCache(opt => opt.AddBasePolicy(builder => builder.Expire(cacheExpiration)));
            services.AddStackExchangeRedisOutputCache(options => options.Configuration = redisConnection);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseOutputCache();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
}
