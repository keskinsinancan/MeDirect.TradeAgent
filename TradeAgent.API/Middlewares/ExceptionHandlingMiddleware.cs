using System.Net;
using System.Text.Json;
using TradeAgent.Logging;

namespace TradeAgent.API.Middlewares
{
	public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, DistributedDemoLogStore logStore)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
		private readonly DistributedDemoLogStore _logStore = logStore;

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				var correlationId = Guid.NewGuid().ToString();
				_logger.LogError(ex, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);
				_logStore.Add($"[API ERROR] CorrelationId={correlationId} | {ex.Message}");

				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";

				var errorResponse = new
				{
					CorrelationId = correlationId,
					Message = "An unexpected error occurred. Please see the logs."
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
			}
		}
	}
}
