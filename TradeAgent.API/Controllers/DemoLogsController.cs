using Microsoft.AspNetCore.Mvc;
using TradeAgent.Logging;

namespace TradeAgent.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DemoLogsController : ControllerBase
	{
		private readonly DistributedDemoLogStore _logStore;

		public DemoLogsController(DistributedDemoLogStore logStore)
		{
			_logStore = logStore;
		}

		[HttpGet]
		public IActionResult GetLogs()
		{
			return Ok(_logStore.GetLogs());
		}
	}
}
