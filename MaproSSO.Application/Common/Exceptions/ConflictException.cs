using System;

namespace MaproSSO.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public string ResourceName { get; }
        public object Key { get; }

        public ConflictException(string resourceName, object key)
            : base($"El recurso '{resourceName}' con clave '{key}' ya existe.")
        {
            ResourceName = resourceName;
            Key = key;
        }

        public ConflictException(string message) : base(message)
        {
        }
    }
}
