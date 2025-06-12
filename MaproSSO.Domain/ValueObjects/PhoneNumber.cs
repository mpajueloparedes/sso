using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace MaproSSO.Domain.ValueObjects
{
    public class PhoneNumber : BaseValueObject
    {
        public string CountryCode { get; private set; }
        public string Number { get; private set; }

        private PhoneNumber() { }

        private PhoneNumber(string countryCode, string number)
        {
            CountryCode = countryCode;
            Number = number;
        }

        public static PhoneNumber Create(string fullNumber)
        {
            if (string.IsNullOrWhiteSpace(fullNumber))
                throw new DomainException("El número de teléfono no puede estar vacío");

            // Limpiar el número
            var cleaned = Regex.Replace(fullNumber, @"[^\d+]", "");

            // Extraer código de país si existe
            string countryCode = "+51"; // Perú por defecto
            string number = cleaned;

            if (cleaned.StartsWith("+"))
            {
                // Buscar el código de país (asumiendo máximo 3 dígitos)
                var match = Regex.Match(cleaned, @"^\+(\d{1,3})(.+)$");
                if (match.Success)
                {
                    countryCode = "+" + match.Groups[1].Value;
                    number = match.Groups[2].Value;
                }
            }

            return new PhoneNumber(countryCode, number);
        }

        public string GetFullNumber() => $"{CountryCode}{Number}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CountryCode;
            yield return Number;
        }

        public override string ToString() => GetFullNumber();
    }
}