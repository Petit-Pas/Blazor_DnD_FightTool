using DnDFightTool.Infrastructure.AspNetCoreExtensions.IoC;
using DnDFightTool.UI.CharacterSheetBlazorComponents.IoC;
using DnDFightTool.Business.DnDActions;
using DnDFightTool.Business.DnDQueries;
using DnDFightTool.UI.DnDQueryPrompter.SaveQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Rolls.Validation;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using DnDFightTool.UI.DnDQueryPrompter;
using DnDFightTool.Infrastructure.Extensions;
using DnDFightTool.Infrastructure.Extensions.IoC;
using DnDFightTool.UI.FightBlazorComponents.IoC;
using DnDFightTool.Domain.Logs.IoC;
using FluentValidation;
using DnDFightTool.Infrastructure.IO.Files;
using DnDFightTool.Infrastructure.IO.Serialization;
using DnDFightTool.Infrastructure.Mapping;
using MudBlazor.Services;
using UndoableMediator.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddValidatorsFromAssemblyContaining<CharacterValidator>();
builder.Services.AddScoped<IValidator<HitRollResult>, HitRollResultValidator>();
builder.Services.AddScoped<IValidator<DamageRollResult>, DamageRollResultValidator>();

var dataFolder = Path.Combine(Path.GetTempPath(), "DnDFightTool.Web");
builder.Services.AddSingleton<ICharacterRepository>(sp =>
    new LocalFileCharacterRepository(sp.GetRequiredService<IFileManager>(), sp.GetRequiredService<IJsonSerializer>(), dataFolder));
builder.Services.AddSingleton<IFightContext, FightContext>();
builder.Services.AddSingleton<ICombatTurnService, CombatTurnService>();
builder.Services.AddSingleton<IAppliedStatusRepository, AppliedStatusRepository>();
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
    .RegisterLogsServices()
    .RegisterCharacterSheetMappingConfigurations()
    .RegisterAspNetCoreExtensions()
    .RegisterCharacterSheetBlazorComponentsServices()
    .RegisterFightBlazorComponentsServices()
    .RegisterPropertyTargetedValidators(typeof(Character).Assembly);

builder.Services.AddSingleton<IDialogServiceProvider, DialogServiceProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DnDFightTool.Components.DndUi.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(DnDFightTool.Components.DndUi.Shared.Components.Routes).Assembly);

app.Run();
