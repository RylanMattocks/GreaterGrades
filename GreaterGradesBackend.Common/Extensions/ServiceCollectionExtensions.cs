using Microsoft.Extensions.DependencyInjection;

namespace GreaterGradesBackend.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
