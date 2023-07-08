using Blazored.Toast;
using DnDEntities.AbilityScores;
using DnDEntities.AttackRolls.ArmorClasses;
using DnDEntities.Characters;
using Microsoft.Extensions.Logging;
using DnDFightTool.Data;
using Morris.Blazor.Validation;
using Blazored.Modal;
using Fight;
using UndoableMediator.DependencyInjection;
using DnDActions;
using FightBlazorComponents.Queries.MartialAttackQueries;
using UndoableMediator.Mediators;

namespace DnDFightTool;

public class Logg : ILogger<IUndoableMediator>
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        throw new NotImplementedException();
    }
}

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
