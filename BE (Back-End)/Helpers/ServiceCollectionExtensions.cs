using System.Reflection;

namespace BE__Back_End_.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Register all repositories and services based on convention
        public static void RegisterServices(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var allTypes = assembly.GetExportedTypes();
            var interfaces = allTypes.Where(t => t.IsInterface && t.Name.StartsWith("I"));
            var implementations = allTypes.Where(t => t.IsClass && !t.IsAbstract);

            foreach (var interfaceType in interfaces)
            {
                var implementationType = implementations.FirstOrDefault(t => t.GetInterfaces().Contains(interfaceType));
                if (implementationType != null)
                {
                    services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                }
            }
        }
    }
}
