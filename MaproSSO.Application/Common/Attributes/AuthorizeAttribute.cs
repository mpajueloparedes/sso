using System;

namespace MaproSSO.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        public string Roles { get; set; }
        public string Permissions { get; set; }
        public bool RequireAll { get; set; } = false;
    }
}