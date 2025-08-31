using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeAgent.Consumer;

class Program
{
	static async Task Main(string[] args)
	{
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		var services = new ServiceCollection();
		services.Configure<RabbitMqOptions>(config.GetSection("RabbitMq"));
		services.AddSingleton<RabbitMqConsumer>();

		var provider = services.BuildServiceProvider();
		var consumer = provider.GetRequiredService<RabbitMqConsumer>();

		await consumer.Start();

		Console.WriteLine("Consumer started. Press [enter] to exit.");
		Console.ReadLine();
	}
}
