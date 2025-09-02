using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Tradeagent.Integration.Tests
{
	public class IntegrationTestFixture : IAsyncLifetime
	{
		public PostgreSqlContainer PostgresContainer { get; }
		public RabbitMqContainer RabbitMqContainer { get; }
		public RedisContainer RedisContainer { get; }

		public string PostgresConnectionString => PostgresContainer.GetConnectionString();
		public string RedisConnectionString => RedisContainer.GetConnectionString();
		public string RabbitMqHost => RabbitMqContainer.Hostname;
		public int RabbitMqPort => RabbitMqContainer.GetMappedPublicPort(5672);

		private readonly string _rabbitUser = "guest";

		private readonly string _rabbitPass = "guest";
		public string RabbitMqUser => _rabbitUser;
		public string RabbitMqPassword => _rabbitPass;

		public IntegrationTestFixture()
		{
			PostgresContainer = new PostgreSqlBuilder()
				.WithDatabase("tradeagent")
				.WithUsername("postgres")
				.WithPassword("postgres")
				.Build();

			RedisContainer = new RedisBuilder().Build();

			RabbitMqContainer = new RabbitMqBuilder()
				.WithUsername("guest")
				.WithPassword("guest")
				.Build();
		}

		public async Task InitializeAsync()
		{
			await PostgresContainer.StartAsync();
			await RedisContainer.StartAsync();
			await RabbitMqContainer.StartAsync();
		}

		public async Task DisposeAsync()
		{
			await RabbitMqContainer.StopAsync();
			await RedisContainer.StopAsync();
			await PostgresContainer.StopAsync();
		}
	}
}
