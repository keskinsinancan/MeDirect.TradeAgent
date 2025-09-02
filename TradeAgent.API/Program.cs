using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;
using TradeAgent.API.Middlewares;
using TradeAgent.API.Workers;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Application.Services;
using TradeAgent.Application.Services.Abstractions;
using TradeAgent.Infrastructure.Data;
using TradeAgent.Infrastructure.Messaging;
using TradeAgent.Infrastructure.Repositories;
using TradeAgent.Infrastructure.Settings;
using TradeAgent.Logging;

namespace TradeAgent.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var configuration = builder.Configuration;
			var services = builder.Services;

			ConfigureServices(services);
			ConfigureApplicationServices(services);
			ConfigureRepositories(services);
			ConfigureDb(services, configuration);
			ConfigureRabbitMq(services, configuration);
			ConfigureRedis(services, configuration);
			ConfigureSerilog(builder);
			builder.Host.UseTradeAgentLogging("TradeAgent.API");
			var app = builder.Build();

			Configure(app, app.Environment);
			app.Run();
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
		}

		private static void ConfigureApplicationServices(IServiceCollection services)
		{
			services.AddScoped<ITradeService, TradeService>();
		}

		private static void ConfigureRepositories(IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, EfUnitOfWork>();
			services.AddScoped<ITradeRepository, TradeRepository>();
			services.AddScoped<IOutboxRepository, OutboxRepository>();
		}

		private static void ConfigureDb(IServiceCollection services, ConfigurationManager configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultDbConnection");

			services.AddDbContext<TradeAgentDbContext>(options =>
				options.UseNpgsql(
					connectionString,
					npgsqlOptions => npgsqlOptions.MigrationsAssembly("TradeAgent.Infrastructure")
				)
			);			
		}

		private static void ConfigureRabbitMq(IServiceCollection services, ConfigurationManager configuration)
		{
			services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
			services.AddHostedService<OutboxPublisher>();
			services.AddSingleton<RabbitMqPublisher>();
		}

		private static void ConfigureRedis(IServiceCollection services, ConfigurationManager configuration)
		{
			services.Configure<RedisOptions>(configuration.GetSection("redis"));
			services.AddSingleton(provider =>
			{
				var options = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
				return new DistributedDemoLogStore(options.REDIS_CONNECTION);
			});
		}

		private static void ConfigureSerilog(WebApplicationBuilder builder)
		{
			builder.Host.UseSerilog((hostContext, services, loggerConfig) =>
			{
				loggerConfig
					.ReadFrom.Configuration(hostContext.Configuration)
					.Enrich.FromLogContext()
					.WriteTo.Console();
			});
		}
		private static void Configure(WebApplication app, IHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseAuthorization();
			app.MapControllers();
			app.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}