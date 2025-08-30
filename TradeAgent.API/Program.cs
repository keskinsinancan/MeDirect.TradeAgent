using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TradeAgent.Infrastructure.Data;

namespace TradeAgent.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			ConfigureServices(builder.Services);
			ConfigureApplicationServices(builder.Services);
			ConfigureRepositories(builder.Services);
			ConfigureDb(builder.Services, builder.Configuration);
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
		}

		private static void ConfigureRepositories(IServiceCollection services)
		{

		}

		private static void ConfigureDb(IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			services.AddDbContext<TradeAgentDbContext>(options =>
				options.UseNpgsql(
					connectionString,
					npgsqlOptions => npgsqlOptions.MigrationsAssembly("TradeAgent.Infrastructure")
				)
			);
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
		}
	}
}