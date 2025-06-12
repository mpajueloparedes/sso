using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.ValueObjects
{
    public class Money : BaseValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new DomainException("El monto no puede ser negativo");

            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("La moneda es requerida");

            return new Money(amount, currency.ToUpperInvariant());
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException("No se pueden sumar montos de diferentes monedas");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException("No se pueden restar montos de diferentes monedas");

            if (Amount < other.Amount)
                throw new DomainException("El resultado no puede ser negativo");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money MultiplyBy(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
