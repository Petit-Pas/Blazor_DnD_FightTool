using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;
using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UndoableMediator.Mediators;

namespace DnDFightTool.Components.DndUi.Shared.Components.Pages;

public partial class FightersPage : IDisposable
{
    private record MonsterGroup(Guid OriginalCharacterId, string TemplateName, int Count, IFightingCharacter Representative);

    private abstract record InFightEntry;
    private sealed record PlayerEntry(IFightingCharacter Fighter) : InFightEntry;
    private sealed record MonsterGroupEntry(MonsterGroup Group) : InFightEntry;

    [Inject]
    public required IUndoableMediator Mediator { get; set; }

    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IDialogServiceProvider DialogServiceProvider { get; set; }

    private string _playerSearch = string.Empty;
    private string _monsterSearch = string.Empty;

    private Character[] _addablePlayers = [];
    private Character[] _addableMonsters = [];
    private InFightEntry[] _inFightEntries = [];

    private Character[] FilteredPlayers => string.IsNullOrWhiteSpace(_playerSearch)
        ? _addablePlayers
        : [.. _addablePlayers.Where(p => p.Name.Contains(_playerSearch, StringComparison.OrdinalIgnoreCase))];

    private Character[] FilteredMonsters => string.IsNullOrWhiteSpace(_monsterSearch)
        ? _addableMonsters
        : [.. _addableMonsters.Where(p => p.Name.Contains(_monsterSearch, StringComparison.OrdinalIgnoreCase))];

    protected override Task OnInitializedAsync()
    {
        DialogServiceProvider.SetDialogService(DialogService);

        FightContext.OnFighterAdded += OnFighterChanged;
        FightContext.OnFighterRemoved += OnFighterChanged;

        RefreshLists();
        return Task.CompletedTask;
    }

    private void OnFighterChanged(object? _, IFightingCharacter __)
    {
        InvokeAsync(() =>
        {
            RefreshLists();
            StateHasChanged();
        });
    }

    private void RefreshLists()
    {
        var characters = CharacterRepository.GetAllCharacters();
        var fighters = FightContext.Fighters.ToArray();
        var inFightPlayerOriginalIds = fighters
            .Where(f => f.Type == CharacterType.Player)
            .Select(f => f.OriginalCharacterId)
            .ToHashSet();

        _addablePlayers = [.. characters
            .Where(c => c.Type == CharacterType.Player)
            .Where(c => !inFightPlayerOriginalIds.Contains(c.Id))
            .OrderBy(c => c.Name)];

        _addableMonsters = [.. characters
            .Where(c => c.Type == CharacterType.Monster)
            .OrderBy(c => c.Name)];

        var sortKey = IFightingCharacter.InitiativeSortKey;

        var playerEntries = fighters
            .Where(f => f.Type == CharacterType.Player)
            .Select(f => (entry: (InFightEntry)new PlayerEntry(f), sortValue: sortKey(f)));

        var monsterEntries = fighters
            .Where(f => f.Type == CharacterType.Monster)
            .GroupBy(f => f.OriginalCharacterId)
            .Select(g =>
            {
                var templateName = CharacterRepository.GetCharacterById(g.Key)?.Name ?? g.First().Name;
                var group = new MonsterGroup(g.Key, templateName, g.Count(), g.First());
                return (entry: (InFightEntry)new MonsterGroupEntry(group), sortValue: sortKey(group.Representative));
            });

        _inFightEntries = [.. playerEntries
            .Concat(monsterEntries)
            .OrderBy(x => x.sortValue)
            .Select(x => x.entry)];
    }

    private async Task OnAddClick(Character character)
    {
        await Mediator.SendAsync(new AddToFightCommand(character.Id));
    }

    private async Task OnRemoveClick(IFightingCharacter fighter)
    {
        await Mediator.SendAsync(new RemoveFromFightCommand(fighter.Id));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FightContext.OnFighterAdded -= OnFighterChanged;
        FightContext.OnFighterRemoved -= OnFighterChanged;
    }
}
