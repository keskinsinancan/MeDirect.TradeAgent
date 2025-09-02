using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Application.DTOs;
using TradeAgent.Application.Services;
using TradeAgent.Application.Services.Abstractions;
using TradeAgent.Infrastructure.Data;
using TradeAgent.Infrastructure.Messaging;
using TradeAgent.Infrastructure.Repositories;
using TradeAgent.Infrastructure.Settings;
using TradeAgent.Logging;

namespace Tradeagent.Integration.Tests
{
	public class TradeAgentIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
	{
		private readonly IntegrationTestFixture _fixture = fixture;
		private ServiceProvider _serviceProvider;

		public async Task InitializeAsync()
		{
			var services = new ServiceCollection();

			// DbContext
			services.AddDbContext<TradeAgentDbContext>(options =>
				options.UseNpgsql(_fixture.PostgresConnectionString));

			// Redis log store
			services.Configure<RedisOptions>(opts => opts.REDIS_CONNECTION = _fixture.RedisConnectionString);
			services.AddSingleton<ILogStore>(provider =>
			{
				var options = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
				return new DistributedDemoLogStore(options.REDIS_CONNECTION);
			});

			// Logger
			services.AddLogging();

			// Repository & Service
			services.AddScoped<ITradeService, TradeService>();
			services.AddScoped<ITradeRepository, TradeRepository>();
			services.AddScoped<IUnitOfWork, EfUnitOfWork>();

			// RabbitMQ publisher
			services.AddSingleton(provider =>
			{
				var rabbitOptions = Options.Create(new RabbitMqOptions
				{
					Host = _fixture.RabbitMqHost,
					Port = _fixture.RabbitMqPort,
					Username = _fixture.RabbitMqUser,
					Password = _fixture.RabbitMqPassword,
					ExchangeName = "test-exchange",
					QueueName = "test-queue"
				});

				var logger = provider.GetRequiredService<ILogger<RabbitMqPublisher>>();
				var logStore = provider.GetRequiredService<ILogStore>();

				return new RabbitMqPublisher(rabbitOptions, logger, logStore);
			});

			_serviceProvider = services.BuildServiceProvider();

			// Ensure database is created
			var db = _serviceProvider.GetRequiredService<TradeAgentDbContext>();
			await db.Database.EnsureCreatedAsync();
		}

		public Task DisposeAsync()
		{
			_serviceProvider?.Dispose();
			return Task.CompletedTask;
		}

		[Fact]
		public async Task TradeService_RecordTradeAsync_Should_Work()
		{
			var tradeService = _serviceProvider.GetRequiredService<ITradeService>();

			var request = new CreateTradeRequest
			{
				AssetName = "Test Asset",
				AssetSymbol = "TST",
				Side = "Buy",
				Quantity = 10,
				Price = 100,
				Currency = "USD",
				CounterpartyId = "CP123123",
				UserId = Guid.NewGuid()
			};

			var result = await tradeService.RecordTradeAsync(request);

			Assert.NotNull(result);
			Assert.Equal(request.AssetName, result.AssetName);
			Assert.Equal(request.Quantity, result.Quantity);
		}

		[Fact]
		public async Task Redis_Should_Store_Logs()
		{
			var logStore = _serviceProvider.GetRequiredService<DistributedDemoLogStore>();
			logStore.Add("Integration test log");

			var redis = await ConnectionMultiplexer.ConnectAsync(_fixture.RedisConnectionString);
			var db = redis.GetDatabase();

			var keys = redis.GetServer(redis.GetEndPoints().First()).Keys();
			Assert.Contains(keys, k => k.ToString().Contains("Integration test log") == false);
		}

		[Fact]
		public async Task RabbitMq_Should_Send_And_Receive_Message()
		{
			var publisher = _serviceProvider.GetRequiredService<RabbitMqPublisher>();
			var message = "Hello Integration Test";

			await publisher.Publish(message, "test-routing");

			Assert.True(true);
		}
	}
}
