using DnDFightTool.Domain.DnDEntities.Characters;
using IO.Files;
using IO.Serialization;
using Mapping;
using Microsoft.Extensions.Logging;
using Morris.Blazor.Validation;
using MudBlazor.Services;
using DnDFightTool.Domain.DnDEntities.IoC;
using AspNetCoreExtensions.IoC;
using DnDEntitiesBlazorComponents.IoC;

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

        builder.Services.AddFormValidation(config =>
        {
            config.AddFluentValidation(
                typeof(Character).Assembly);
        });

        builder.Services.AddSingleton<ICharacterRepository, LocalFileCharacterRepository>();
        builder.Services.AddSingleton<IFileManager, LocalFileManager>();
        builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();

        builder.Services.AddSingleton<IMapper, Mapper>();
        
        builder.Services
            .RegisterDnDEntitiesMappingConfigurations()
            .RegisterAspNetCoreExtensions()
            .RegisterDnDEntitiesBlazorComponentsServices();

        var app = builder.Build();
        return app;
	}
}
