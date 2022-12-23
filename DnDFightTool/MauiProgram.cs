using Microsoft.Extensions.Logging;
using DnDFightTool.Data;
using FluentValidation;
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

        builder.Services.AddSingleton<IValidator<CharacterDummy>, CharacterDummyValidator>();
        builder.Services.AddSingleton<IValidator<CharacteristicDummy>, CharacteristicDummyValidator>();

        builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
