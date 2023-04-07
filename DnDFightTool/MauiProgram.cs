using Blazored.Toast;
using DnDEntities.AbilityScores;
using DnDEntities.AttackRolls.ArmorClasses;
using DnDEntities.Characters;
using Microsoft.Extensions.Logging;
using DnDFightTool.Data;
using Morris.Blazor.Validation;
using static DnDEntitiesBlazorComponents.CharacterSheet;
using Blazored.Modal;

namespace DnDFightTool;

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

        builder.Services.AddBlazoredToast();
        builder.Services.AddBlazoredModal();

        builder.Services.AddSingleton<ICharacterRepository, InMemoryCharacterRepository>();
        builder.Services.AddSingleton<IFileManager, FileManager>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddFormValidation(config =>
            {
                config.AddFluentValidation(
                    typeof(AbilityScore).Assembly, 
                    typeof(ArmorClass).Assembly);
            }
        );


        builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
