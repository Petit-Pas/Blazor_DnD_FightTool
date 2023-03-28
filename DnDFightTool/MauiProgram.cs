using Characters.AbilityScores;
using Microsoft.Extensions.Logging;
using DnDFightTool.Data;
using Fight.AttackRolls.ArmorClasses;
using Morris.Blazor.Validation;
using static DnDBlazorComponents.CharacterSheet;

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
