using Microsoft.AspNetCore.Mvc;
using TradeAgent.Application.DTOs;
using TradeAgent.Application.Services.Abstractions;

namespace TradeAgent.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public sealed class TradesController : ControllerBase
	{
		private readonly ITradeService _tradeService;

		public TradesController(ITradeService tradeService)
		{
			_tradeService = tradeService;
		}

		[HttpPost]
		public async Task<ActionResult<TradeDto>> RecordTrade([FromBody] CreateTradeRequest request, CancellationToken cancellationToken)
		{
			var result = await _tradeService.RecordTradeAsync(request, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<TradeDto>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var result = await _tradeService.GetTradeByIdAsync(id, cancellationToken);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<TradeDto>>> GetAll(CancellationToken cancellationToken)
		{
			var results = await _tradeService.GetAllTradesAsync(cancellationToken);
			return Ok(results);
		}
	}
}
