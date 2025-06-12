namespace MaproSSO.Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; }
        public object Key { get; }

        public EntityNotFoundException(string entityName, object key)
            : base($"Entidad '{entityName}' con clave '{key}' no fue encontrada.")
        {
            EntityName = entityName;
            Key = key;
        }

        public EntityNotFoundException(string message) : base(message) { }
    }
}