#region

using System.Reflection;
using LeChuck.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace LeChuck.Stateless.StateMachine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateMachines(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddInherited<IStateMachine>(assemblies);
            services.AddSingleton<IStateMachineFactory, StateMachineFactory>();
            return services;
        }
    }
}