using DnDFightTool.Infrastructure.AspNetCoreExtensions.IoC;
using DnDFightTool.UI.CharacterSheetBlazorComponents.IoC;
using DnDFightTool.Business.DnDActions;
using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.Fight;
using DnDFightTool.UI.DnDQueryPrompter;
using DnDFightTool.UI.DnDQueryPrompter.SaveQueries;
using DnDFightTool.Infrastructure.Extensions;
using DnDFightTool.Infrastructure.Extensions.IoC;
using DnDFightTool.UI.FightBlazorComponents.IoC;
using FluentValidation;
using DnDFightTool.Infrastructure.IO.Files;
using DnDFightTool.Infrastructure.IO.Serialization;
using DnDFightTool.Infrastructure.Mapping;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using UndoableMediator.DependencyInjection;


namespace DnDFightTool.Components.DndUi;

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
}
