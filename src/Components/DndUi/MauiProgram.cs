using System.Reflection;
using AspNetCoreExtensions.IoC;
using DnDEntitiesBlazorComponents.IoC;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Characters.Validation;
using DnDFightTool.Domain.DnDEntities.IoC;
using DnDFightTool.Domain.Fight;
using Extensions;
using FluentValidation;
using IO.Files;
using IO.Serialization;
using Mapping;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

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
        //builder.Services.AddFormValidation(config =>
        //{
        //    config.AddFluentValidation(
        //        typeof(Character).Assembly);
        //});

        builder.Services.AddSingleton<ICharacterRepository, LocalFileCharacterRepository>();
        builder.Services.AddSingleton<IFightContext, FightContext>();
        builder.Services.AddSingleton<IFileManager, LocalFileManager>();
        builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();

        builder.Services.AddSingleton<IMapper, Mapper>();
        
        builder.Services
            .RegisterDnDEntitiesMappingConfigurations()
            .RegisterAspNetCoreExtensions()
            .RegisterDnDEntitiesBlazorComponentsServices()
            .RegisterPropertyTargetedValidators(typeof(Character).Assembly);

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
            services.AddTransient(x.Base, x.Type);
        }

        return services;
    }
}
