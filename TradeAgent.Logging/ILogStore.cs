namespace TradeAgent.Logging
{
	public interface ILogStore
	{
		void Add(string message);
		string[] GetLogs();
	}
}
