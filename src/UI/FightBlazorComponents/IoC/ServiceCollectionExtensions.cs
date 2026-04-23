using DnDFightTool.Domain.Rolls.Validation;
using FightBlazorComponents.Entities.Dices.DiceRolls;
using Microsoft.Extensions.DependencyInjection;
using SharedComponents.Dices;

namespace FightBlazorComponents.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterFightBlazorComponentsServices(this IServiceCollection services)
    {
        services.AddScoped<IDiceRollNotifier, DiceRollNotifier>();
        services.AddScoped<DiceRollResultValidator>();

        return services;
    }
}
