using DnDEntitiesBlazorComponents;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using Mapping;
using Microsoft.AspNetCore.Components;

namespace DndUi.Shared.Components.Pages;

public partial class CharacterListEditorPage
{
    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IGlobalEditContext GlobalEditContext { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required IFightContext FightContext { get; set; }

    private Character[] _players = [];
    private Character[] _monsters = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ResetCharacters();
    }

    private void ResetCharacters()
    {
        var characters = CharacterRepository.GetAllCharacters().ToArray();
        _players = [.. characters.Where(x => x.Type is CharacterType.Player)];
        _monsters = [.. characters.Where(x => x.Type is CharacterType.Monster)];

        StateHasChanged();
    }

    private void AddToFight(Character character)
    {
        FightContext.Add(character);
    }

    private void Edit(Character character)
    {
        var copy = Mapper.Copy(character);
        GlobalEditContext.EditCharacter(copy);
    }

    private void Duplicate(Character character)
    {
        var newCharacter = Mapper.Clone(character);
        GlobalEditContext.EditCharacter(newCharacter);
    }

    private void Delete(Character character)
    {
        CharacterRepository.Delete(character);
        ResetCharacters();
    }

    private void AddNew(CharacterType type)
    {
        var newCharacter = new Character(true) { Type = type };
        GlobalEditContext.EditCharacter(newCharacter);
    }
}
