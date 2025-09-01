using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace TradeAgent.Logging
{
	public static class LoggingExtensions
	{
		public static IHostBuilder UseTradeAgentLogging(this IHostBuilder hostBuilder, string applicationName)
		{
			return hostBuilder.UseSerilog((context, configuration) =>
			{
				configuration
					.Enrich.FromLogContext()
					.Enrich.WithProperty("Application", applicationName)
					.WriteTo.Console(new RenderedCompactJsonFormatter());
			});
		}
	}
}
