namespace MaproSSO.Domain.Exceptions
{
    public class BusinessRuleValidationException : DomainException
    {
        public string Details { get; }

        public BusinessRuleValidationException(string message) : base(message) { }

        public BusinessRuleValidationException(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}
