using TradeAgent.Domain.Common;

namespace TradeAgent.Domain.ValueObjects
{
	public sealed class Money : ValueObject
	{
		public decimal Amount { get; }
		public string Currency { get; }

		private Money(decimal amount, string currency)
		{
			if (amount < 0)
				throw new ArgumentException("Amount can not be negative");

			if (string.IsNullOrWhiteSpace(currency))
				throw new ArgumentException("Currency can not be empty");

			Amount = amount;
			Currency = currency.Trim();
		}

		public static Money Create(decimal amount, string currency)
		{
			return new Money(amount, currency);
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Amount;
			yield return Currency;
		}

		public override string ToString()
		{
			return $"{Amount} {Currency}";
		}
	}
}
