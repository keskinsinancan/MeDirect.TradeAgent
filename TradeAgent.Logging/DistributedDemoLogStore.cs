using StackExchange.Redis;

namespace TradeAgent.Logging
{
	public class DistributedDemoLogStore : ILogStore
	{
		private readonly IDatabase _db;
		private const string Key = "DemoLogs";
		private const int MaxEntries = 100;

		public DistributedDemoLogStore(string redisConnection)
		{
			var redis = ConnectionMultiplexer.Connect(redisConnection);
			_db = redis.GetDatabase();
		}

		public void Add(string message)
		{
			var log = $"{DateTime.UtcNow:O} - {message}";
			_db.ListRightPush(Key, log);
			_db.ListTrim(Key, -MaxEntries, -1);
		}

		public string[] GetLogs()
		{
			var logs = _db.ListRange(Key, 0, -1);
			return [.. logs.Select(x => x.ToString()).Reverse().Take(10)];
		}
	}
}
