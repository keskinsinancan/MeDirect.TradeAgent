using TradeAgent.Domain.Common;

namespace TradeAgent.Domain.ValueObjects
{
	public sealed class Asset : ValueObject
	{
		public string Name { get; }
		public string Symbol { get; }

		private Asset(string name, string symbol)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} can not be null!");
			}

			if (string.IsNullOrWhiteSpace(symbol))
			{
				throw new ArgumentException($"{nameof(symbol)} can not be null!");
			}

			Name = name.Trim();
			Symbol = symbol.Trim();
		}

		public static Asset Create(string name, string symbol)
		{
			return new Asset(name, symbol);
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Name;
			yield return Symbol;
		}
	}
}
