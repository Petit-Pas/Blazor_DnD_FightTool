using Blazored.Toast;
using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.Extensions.Logging;
using DnDFightTool.Data;
using Morris.Blazor.Validation;
using Blazored.Modal;
using DnDFightTool.Domain.Fight;
using UndoableMediator.DependencyInjection;
using DnDFightTool.Business.DnDActions;
using FightBlazorComponents.Queries.MartialAttackQueries;

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
        builder.Services.AddSingleton<IFightContext, FightContext>();
        builder.Services.AddSingleton<IFileManager, FileManager>();
        builder.Services.AddSingleton<IAppliedStatusCollection, AppliedStatusCollection>();

        builder.Services.ConfigureMediator(options =>
        {
            options.ShouldScanAutomatically = false;
            options.AssembliesToScan = new System.Reflection.Assembly[]
            {
                typeof(CasterCommandBase).Assembly,
                typeof(MartialAttackRollResultQueryHandler).Assembly
            };
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddFormValidation(config =>
            {
                config.AddFluentValidation(
                    typeof(Character).Assembly);
            }
        );


        builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
