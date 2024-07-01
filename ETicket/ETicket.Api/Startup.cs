
using Azure.Messaging.ServiceBus;
using ETicket.Api.HealthCheck;
using ETicket.Bll.Services;
using ETicket.Bll.Services.Caching;
using ETicket.Bll.Services.Cart;
using ETicket.Bll.Services.Notifications;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Abstractions;
using IWent.Services.Notifications.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
                //.AddUserSecrets<Startup>()
                .Build();

            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connection =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" ?
                Configuration.GetConnectionString("ETicketDbProd") :
                Configuration.GetConnectionString("ETicketDb");

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

            services.AddTransient(services =>
            {
                var configuration = services.GetRequiredService<IConfiguration>();
                return configuration.GetRequiredSection("ServiceBus").Get<ServiceBusConfiguration>()
                    ?? throw new InvalidOperationException($"Unable to get the '{typeof(ServiceBusConfiguration)}' from configuration.");
            });

            services.AddSingleton(typeof(ICacheService), typeof(CacheService));

            var cacheExpiration = Configuration.GetRequiredSection("Caching").Get<CacheConfiguration>().SlidingExpirationTimeSpan;
            services.AddOutputCache(opt => opt.AddBasePolicy(builder => builder.Expire(cacheExpiration)));
            services.AddStackExchangeRedisOutputCache(options => options.Configuration = redisConnection);

            var serviceBusConnectionString = Configuration.GetRequiredSection("ServiceBus").Get<ServiceBusConfiguration>().ConnectionString;
            services.AddSingleton(provider =>
            {
                return new ServiceBusClient(serviceBusConnectionString);
            });

            services.AddSingleton<INotificationProducer, NotificationProducer>();

            services.AddHealthChecks()
                .AddCheck("SqlServerHealthCheck", new SqlServerHealthCheck(connection))
                .AddCheck("ServiceBusQueueHealthCheck", new AzureServiceBusHealthCheck(serviceBusConnectionString,
                 Configuration.GetRequiredSection("ServiceBus").Get<ServiceBusConfiguration>().QueueName), HealthStatus.Unhealthy);

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            status = report.Status.ToString(),
                            applicationVersion = typeof(Startup).Assembly.GetName().Version.ToString(),
                            dependencies = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status.ToString(),
                                description = e.Value.Description
                            }).ToArray()
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });
            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
}
