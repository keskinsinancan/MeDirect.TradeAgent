using Microsoft.AspNetCore.Mvc;
using TradeAgent.Logging;

namespace TradeAgent.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DemoLogsController(ILogStore logStore) : ControllerBase
	{
		private readonly ILogStore _logStore = logStore;

		[HttpGet]
		public IActionResult GetLogs()
		{
			return Ok(_logStore.GetLogs());
		}
	}
}
