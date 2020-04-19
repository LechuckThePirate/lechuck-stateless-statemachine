using LeChuck.StateMachine;
using Microsoft.Extensions.DependencyInjection;

namespace LeChuck.Stateless.StateMachine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateMachines(this IServiceCollection services)
        {
            services.AddSingleton<IStateMachineFactory, StateMachineFactory>();
            return services;
        }
    }
}
