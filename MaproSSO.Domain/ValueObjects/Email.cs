using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace MaproSSO.Domain.ValueObjects
{
    public class Email : BaseValueObject
    {
        public string Value { get; private set; }

        private Email() { }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El email no puede estar vacío");

            value = value.Trim().ToLowerInvariant();

            if (!IsValid(value))
                throw new DomainException("El formato del email no es válido");

            return new Email(value);
        }

        private static bool IsValid(string email)
        {
            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email?.Value;
    }
}