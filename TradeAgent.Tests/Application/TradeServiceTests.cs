using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Application.DTOs;
using TradeAgent.Application.Services;
using TradeAgent.Application.Services.Abstractions;
using TradeAgent.Domain.Entites;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;
using TradeAgent.Logging;

namespace TradeAgent.Tests.Application
{
	public class TradeServiceTests
	{
		private readonly Mock<ITradeRepository> _tradeRepositoryMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<TradeService>> _loggerMock;
		private readonly Mock<ILogStore> _logStoreMock;
		private readonly TradeService _service;

		public TradeServiceTests()
		{
			_tradeRepositoryMock = new Mock<ITradeRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<TradeService>>();
			_logStoreMock = new Mock<ILogStore>();

			_service = new TradeService(
				_tradeRepositoryMock.Object,
				_unitOfWorkMock.Object,
				_loggerMock.Object,
				_logStoreMock.Object
			);
		}

		[Fact]
		public async Task RecordTradeAsync_Should_Save_Trade_And_Return_Dto()
		{
			var request = GetCreateTradeRequest();
			// Act
			var result = await _service.RecordTradeAsync(request, CancellationToken.None);

			// Assert
			result.Should().NotBeNull();
			result.AssetName.Should().Be("Bitcoin");
			result.AssetSymbol.Should().Be("BTC");

			_tradeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Trade>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			_logStoreMock.Verify(l => l.Add(It.Is<string>(s => s.Contains("[TRADE] Executing trade"))), Times.Once);
		}

		[Fact]
		public async Task RecordTradeAsync_Should_LogError_When_Exception_Occurs()
		{
			// Arrange
			var request = GetCreateTradeRequest();

			_tradeRepositoryMock
				.Setup(r => r.AddAsync(It.IsAny<Trade>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			Func<Task> act = async () => await _service.RecordTradeAsync(request);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("DB error");

			_loggerMock.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Failed to record trade")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()
				),
				Times.Once);

			_logStoreMock.Verify(l => l.Add(It.Is<string>(s => s.Contains("[TRADE ERROR]"))), Times.Once);
		}

		[Fact]
		public async Task GetAllTradesAsync_Should_Return_All_Trades()
		{
			// Arrange
			var price = Money.Create(25000, "USD");
			var trades = new List<Trade>
			{
				Trade.Execute(Asset.Create("BTC", "BTC"), TradeSide.Buy, 1, price, "CP12313", Guid.NewGuid(), DateTime.UtcNow),
				Trade.Execute(Asset.Create("ETH", "ETH"), TradeSide.Sell, 2, price, "CP12313", Guid.NewGuid(), DateTime.UtcNow)
			};

			_tradeRepositoryMock
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(trades);

			// Act
			var result = await _service.GetAllTradesAsync();

			// Assert
			result.Should().HaveCount(2);
			_loggerMock.Verify(
				x => x.Log
				(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Retrieved all trades. Count={trades.Count}")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()
				),
				Times.Once);
		}

		[Fact]
		public async Task GetAllTradesAsync_Should_LogError_When_Exception_Occurs()
		{
			// Arrange
			_tradeRepositoryMock
				.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			Func<Task> act = async () => await _service.GetAllTradesAsync();

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("DB error");

			_loggerMock.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to retrieve all trades")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);

			_logStoreMock.Verify(l => l.Add(It.Is<string>(s => s.Contains("[TRADE ERROR]"))), Times.Once);
		}

		[Fact]
		public async Task GetTradeByIdAsync_Should_Return_Trade_When_Found()
		{
			// Arrange
			var price = Money.Create(25000, "USD");
			var trade = Trade.Execute(Asset.Create("BTC", "BTC"), TradeSide.Buy, 1, price, "CP12313", Guid.NewGuid(), DateTime.UtcNow);
			_tradeRepositoryMock
				.Setup(r => r.GetByIdAsync(trade.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(trade);

			// Act
			var result = await _service.GetTradeByIdAsync(trade.Id);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(trade.Id);
			_loggerMock.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Retrieved trade by Id. TradeId={trade.Id}")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[Fact]
		public async Task GetTradeByIdAsync_Should_Return_Null_When_NotFound()
		{
			// Arrange
			var tradeId = Guid.NewGuid();
			_tradeRepositoryMock
				.Setup(r => r.GetByIdAsync(tradeId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Trade?)null);

			// Act
			var result = await _service.GetTradeByIdAsync(tradeId);

			// Assert
			result.Should().BeNull();
			_loggerMock.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => true), // Burada mesajın içeriğini kontrol etmiyoruz
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Never
			);
		}

		[Fact]
		public async Task GetTradeByIdAsync_Should_LogError_When_Exception_Occurs()
		{
			// Arrange
			var tradeId = Guid.NewGuid();
			_tradeRepositoryMock
				.Setup(r => r.GetByIdAsync(tradeId, It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			Func<Task> act = async () => await _service.GetTradeByIdAsync(tradeId);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("DB error");

			_loggerMock.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to retrieve trade by Id")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);

			_logStoreMock.Verify(l => l.Add(It.Is<string>(s => s.Contains("[TRADE ERROR]"))), Times.Once);
		}

		private static CreateTradeRequest GetCreateTradeRequest()
		{
			return new CreateTradeRequest
			{
				AssetName = "Bitcoin",
				AssetSymbol = "BTC",
				Side = "Buy",
				Quantity = 1.5m,
				Price = 25000,
				Currency = "USD",
				CounterpartyId = "CP12313",
				UserId = Guid.NewGuid()
			};
		}
	}

}
