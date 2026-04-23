using DnDFightTool.Domain.Rolls.Validation;
using DnDFightTool.UI.FightBlazorComponents.Entities.Dices.DiceRolls;
using Microsoft.Extensions.DependencyInjection;
using DnDFightTool.UI.SharedComponents.Dices;

namespace DnDFightTool.UI.FightBlazorComponents.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterFightBlazorComponentsServices(this IServiceCollection services)
    {
        services.AddScoped<IDiceRollNotifier, DiceRollNotifier>();
        services.AddScoped<DiceRollResultValidator>();

        return services;
    }
}
