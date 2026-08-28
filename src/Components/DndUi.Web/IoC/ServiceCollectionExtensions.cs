using DnDFightTool.Business.DnDActions;
using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using DnDFightTool.Domain.Logs.IoC;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Rolls.Validation;
using DnDFightTool.Infrastructure.AspNetCoreExtensions.IoC;
using DnDFightTool.Infrastructure.Extensions.IoC;
using DnDFightTool.Infrastructure.IO.Files;
using DnDFightTool.Infrastructure.IO.Serialization;
using DnDFightTool.Infrastructure.Mapping;
using DnDFightTool.UI.CharacterSheetBlazorComponents.IoC;
using DnDFightTool.UI.DnDQueryPrompter;
using DnDFightTool.UI.DnDQueryPrompter.SaveQueries;
using DnDFightTool.UI.FightBlazorComponents.IoC;
using FluentValidation;
using MudBlazor.Services;
using UndoableMediator.DependencyInjection;

namespace DnDFightTool.Components.DndUi.Web.IoC;

/// <summary>
///     Composes the service graph of the <c>DndUi.Web</c> host.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers every service the web host needs, from the Blazor Server render mode down to the domain singletons.
    /// </summary>
    /// <param name="services">The collection to register into.</param>
    /// <param name="dataFolder">
    ///     Folder the <see cref="LocalFileCharacterRepository"/> persists characters to. It is read eagerly the first time
    ///     <see cref="ICharacterRepository"/> is resolved, not at registration time.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection RegisterWebAppServices(this IServiceCollection services, string dataFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataFolder);

        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddMudServices();

        services.AddValidatorsFromAssemblyContaining<CharacterValidator>();
        // Both scans above only reach the CharacterSheet assembly, so the Rolls validators are registered by hand.
        services.AddScoped<IValidator<HitRollResult>, HitRollResultValidator>();
        services.AddScoped<IValidator<DamageRollResult>, DamageRollResultValidator>();

        services.AddSingleton<ICharacterRepository>(sp =>
            new LocalFileCharacterRepository(sp.GetRequiredService<IFileManager>(), sp.GetRequiredService<IJsonSerializer>(), dataFolder));
        services.AddSingleton<IFightContext, FightContext>();
        services.AddSingleton<ICombatTurnService, CombatTurnService>();
        services.AddSingleton<IAppliedStatusRepository, AppliedStatusRepository>();
        services.AddSingleton<IFileManager, LocalFileManager>();
        services.AddSingleton<IJsonSerializer, JsonSerializer>();

        services.AddSingleton<IMapper, Mapper>();

        services.ConfigureMediator(options =>
        {
            options.ShouldScanAutomatically = false;
            options.AssembliesToScan =
            [
                typeof(CasterCommandBase).Assembly,
                typeof(SaveRollResultQueryHandler).Assembly
            ];
        });

        services
            .RegisterLogsServices()
            .RegisterCharacterSheetMappingConfigurations()
            .RegisterAspNetCoreExtensions()
            .RegisterCharacterSheetBlazorComponentsServices()
            .RegisterFightBlazorComponentsServices()
            .RegisterPropertyTargetedValidators(typeof(Character).Assembly);

        services.AddSingleton<IDialogServiceProvider, DialogServiceProvider>();

        return services;
    }
}
