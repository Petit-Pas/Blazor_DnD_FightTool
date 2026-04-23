using System.Reflection;
using AspNetCoreExtensions.IoC;
using CharacterSheetBlazorComponents.IoC;
using DnDFightTool.Business.DnDActions;
using DnDFightTool.Business.DnDQueries;
using DnDQueryPrompter.SaveQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Rolls.Validation;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.Fight;
using DnDQueryPrompter;
using Extensions;
using FightBlazorComponents.IoC;
using FluentValidation;
using IO.Files;
using IO.Serialization;
using Mapping;
using MudBlazor.Services;
using UndoableMediator.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddValidatorsFromAssemblyContaining<CharacterValidator>();
builder.Services.AddScoped<AbstractValidator<HitRollResult>, HitRollResultValidator>();
builder.Services.AddScoped<AbstractValidator<DamageRollResult>, DamageRollResultValidator>();

var dataFolder = Path.Combine(Path.GetTempPath(), "DnDFightTool.Web");
builder.Services.AddSingleton<ICharacterRepository>(sp =>
    new LocalFileCharacterRepository(sp.GetRequiredService<IFileManager>(), sp.GetRequiredService<IJsonSerializer>(), dataFolder));
builder.Services.AddSingleton<IFightContext, FightContext>();
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

app.MapRazorComponents<DndUi.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(DndUi.Shared.Components.Routes).Assembly);

app.Run();

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPropertyTargetedValidators(this IServiceCollection services, Assembly assembly)
    {
        var openGenericType = typeof(PropertyTargetedValidator<>);

        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Type = t,
                Base = t.BaseType
            })
            .Where(x => x.Base != null
                && x.Base.IsGenericType
                && x.Base.GetGenericTypeDefinition() == openGenericType)
            .ToList();

        foreach (var x in types)
        {
            services.AddTransient(x.Base!, x.Type);
        }

        return services;
    }
}
