using TradeAgent.Consumer;

class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Starting TradeAgent.Consumer...");

		var consumer = new RabbitMqConsumer("localhost", "trade_events_queue");
		await consumer.Start();

		Console.WriteLine("Consumer started. Press [enter] to exit.");
		Console.ReadLine();
	}
}