using Blazored.Toast;
using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.Extensions.Logging;
using Morris.Blazor.Validation;
using Blazored.Modal;
using DnDFightTool.Domain.Fight;
using UndoableMediator.DependencyInjection;
using DnDFightTool.Business.DnDActions;
using FightBlazorComponents.Queries.MartialAttackQueries;
using IO.Files;

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

        builder.Services.AddSingleton<ICharacterRepository, LocalFileCharacterRepository>();
        builder.Services.AddSingleton<IFightContext, FightContext>();
        builder.Services.AddSingleton<IFileManager, LocalFileManager>();
        builder.Services.AddSingleton<IAppliedStatusRepository, AppliedStatusRepository>();

        builder.Services.ConfigureMediator(options =>
        {
            options.ShouldScanAutomatically = false;
            options.AssembliesToScan =
            [
                typeof(CasterCommandBase).Assembly,
                typeof(MartialAttackRollResultQueryHandler).Assembly
            ];
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

		return builder.Build();
	}
}
