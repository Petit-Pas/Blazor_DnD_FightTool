using System.Reflection;
using AspNetCoreExtensions.IoC;
using CharacterSheetBlazorComponents.IoC;
using DnDFightTool.Business.DnDActions;
using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.Fight;
using DnDQueryPrompter;
using DnDQueryPrompter.SaveQueries;
using Extensions;
using FightBlazorComponents.IoC;
using FluentValidation;
using IO.Files;
using IO.Serialization;
using Mapping;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using UndoableMediator.DependencyInjection;


namespace DndUi;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices(); // MudBlazor registration

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddValidatorsFromAssemblyContaining<CharacterValidator>();

        builder.Services.AddSingleton<IAppliedStatusRepository, AppliedStatusRepository>();
        builder.Services.AddSingleton<ICharacterRepository, LocalFileCharacterRepository>();
        builder.Services.AddSingleton<IFightContext, FightContext>();
        builder.Services.AddSingleton<IFileManager, LocalFileManager>();
        builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();

        builder.Services.AddSingleton<IMapper, Mapper>();

        builder.Services.ConfigureMediator(options =>
        {
            options.ShouldScanAutomatically = false;
            options.AssembliesToScan =
            [
                typeof(CasterCommandBase).Assembly,
                typeof(SaveRollResultQueryHandler).Assembly
            ];
        });

        builder.Services
            .RegisterCharacterSheetMappingConfigurations()
            .RegisterAspNetCoreExtensions()
            .RegisterCharacterSheetBlazorComponentsServices()
            .RegisterFightBlazorComponentsServices()
            .RegisterPropertyTargetedValidators(typeof(Character).Assembly);

        builder.Services.AddSingleton<IDialogServiceProvider, DialogServiceProvider>();

        

        var app = builder.Build();

        return app;
	}

    public static IServiceCollection RegisterPropertyTargetedValidators(this IServiceCollection services, Assembly assembly)
    {
        var openGenericType = typeof(PropertyTargetedValidator<>);

        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Type = t,
                Base = t.BaseType
            })
            .Where(x => x.Base != null
                && x.Base.IsGenericType
                && x.Base.GetGenericTypeDefinition() == openGenericType)
            .ToList();

        foreach (var x in types)
        {
            services.AddTransient(x.Base!, x.Type);
        }

        return services;
    }
}
