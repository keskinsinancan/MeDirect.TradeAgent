using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using TradeAgent.Consumer;
using TradeAgent.Logging;

class Program
{
	static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.UseTradeAgentLogging("TradeAgent.Consumer")
			.ConfigureAppConfiguration((context, config) =>
			{
				config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			})
			.ConfigureServices((context, services) =>
			{
				services.Configure<RabbitMqOptions>(context.Configuration.GetSection("RabbitMq"));
				services.AddSingleton<RabbitMqConsumer>();
				services.AddSingleton<DistributedDemoLogStore>();
				services.Configure<RedisOptions>(context.Configuration.GetSection("Redis"));
				services.AddSingleton<ILogStore>(provider =>
				{
					var options = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
					return new DistributedDemoLogStore(options.REDIS_CONNECTION);
				});
			})
			.Build();

		var consumer = host.Services.GetRequiredService<RabbitMqConsumer>();
		await consumer.Start();

		Log.Information("Consumer started. Press [enter] to exit.");
		Console.ReadLine();
	}
}
