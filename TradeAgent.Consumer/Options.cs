namespace TradeAgent.Consumer
{
	public class RabbitMqOptions
	{
		public string Host { get; set; } = default!;
		public int Port { get; set; }
		public string Username { get; set; } = default!;
		public string Password { get; set; } = default!;
		public string ExchangeName { get; set; } = default!;
		public string QueueName { get; set; } = default!;
		public string RoutingKey { get; set; } = default!;
	}

	public class RedisOptions
	{
		public string REDIS_CONNECTION { get; set; } = default!;
	}
}
