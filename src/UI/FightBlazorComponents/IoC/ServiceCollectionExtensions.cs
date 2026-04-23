using FightBlazorComponents.Entities.Dices.DiceRolls;
using Microsoft.Extensions.DependencyInjection;
using SharedComponents.Dices;

namespace FightBlazorComponents.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterFightBlazorComponentsServices(this IServiceCollection services)
    {
        services.AddScoped<IDiceRollNotifier, DiceRollNotifier>();

        return services;
    }
}
