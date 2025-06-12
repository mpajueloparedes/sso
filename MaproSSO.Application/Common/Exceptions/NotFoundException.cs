using System;

namespace MaproSSO.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string EntityName { get; }
        public object Key { get; }

        public NotFoundException(string entityName, object key)
            : base($"La entidad '{entityName}' con clave '{key}' no fue encontrada.")
        {
            EntityName = entityName;
            Key = key;
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}