
using Blazored.Modal;
using Blazored.Modal.Services;
using DnDEntities.Characters;
using Fight;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NeoBlazorphic.StyleParameters;
using SharedComponents.Modals.ConfirmationModals;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Pages;

public partial class AllCharactersPage
{
    [Inject]
    public ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    [Inject]
    public IModalService Modal { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; }

    private Character[] _players;

    private Character[] _monsters;

    private CharacterType _typeDisplayed { get; set; } = CharacterType.Player;

    private static readonly BorderRadius HeaderBorderRadius = new BorderRadius(0, 0, 1, 1, "rem");
    private static readonly BorderRadius FooterBorderRadius = new BorderRadius(1, 1, 0, 0, "rem");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        var characters = CharacterRepository.GetAllCharacters().ToArray();
        _players = characters.Where(x => x.Type is CharacterType.Player).ToArray();
        _monsters = characters.Where(x => x.Type is CharacterType.Monster).ToArray();
    }

    private void SwitchType(MouseEventArgs _)
    {
        _typeDisplayed = _typeDisplayed == CharacterType.Player ? CharacterType.Monster : CharacterType.Player;
    }

    private void CreateNew(MouseEventArgs _)
    {
        Navigation.NavigateTo($"characters/edit/{(_typeDisplayed is CharacterType.Player ? "newPlayer" : "newMonster")}");
    }

    private async Task Delete(Character character)
    {
        var parameters = new ModalParameters().Add(nameof(ConfirmationModal.Message), "Hello world message");
        var options = new ModalOptions { UseCustomLayout = true };
        var modal = Modal.Show<ConfirmationModal>("Default Title?", parameters, options);
        var result = await modal.Result;
        
        if (result.Confirmed)
        {
            CharacterRepository.Delete(character);
            if (character.Type is CharacterType.Player)
            {
                _players = _players.Where(x => x.Id != character.Id).ToArray();
            }
            else
            {
                _monsters = _monsters.Where(x => x.Id != character.Id).ToArray();
            }
        }
    }

    private void AddToFight(Character character)
    {
        FightContext.AddToFight(character);
    }

    private void Edit(Character character)
    {
        Navigation.NavigateTo($"characters/edit/{character.Id}");
    }

    // UI Methods
    private Character[] SelectedCharacters => _typeDisplayed switch
    {
        CharacterType.Player => _players,
        CharacterType.Monster => _monsters,
        _ => throw new IndexOutOfRangeException()
    };
}