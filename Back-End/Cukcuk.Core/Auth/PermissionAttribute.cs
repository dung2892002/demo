namespace Cukcuk.Core.Auth
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute(string permission) : Attribute
    {
        public string Permission { get; } = permission;
    }
}

