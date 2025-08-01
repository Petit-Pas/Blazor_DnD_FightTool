using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.Extensions.Logging;
using Morris.Blazor.Validation;
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

        builder.Services.AddFormValidation(config =>
        {
            config.AddFluentValidation(
                typeof(Character).Assembly);
        });

        return builder.Build();
	}
}
