using Blazored.Toast.Services;
using DnDFightTool.Domain.DnDEntities.Characters;
using FastDeepCloner;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Pages;

public partial class EditCharacterPage
{
    [Parameter]
    public string CharacterId { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }
    [Inject]
    public IToastService ToastService { get; set; }


    [Inject]
    public ICharacterRepository CharacterRepository { get; set; }

    private Character? _character { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (CharacterId == "newPlayer")
        {
            _character = new Character(true)
            {
                Name = "new character",
                Type = CharacterType.Player
            };
        }
        else if (CharacterId == "newMonster")
        {
            _character = new Character(true)
            {
                Name = "new character",
                Type = CharacterType.Monster
            };
        }
        else {
            var id = Guid.Parse(CharacterId);
            _character = CharacterRepository.GetCharacterById(id).Clone();
        }
    }

    private void SaveClicked()
    {
        CharacterRepository.Save(_character!);
        ToastService.ShowError($"Character {_character?.Name} has been successfully saved!");
        Navigation.NavigateTo("/Characters");
    }

    private void CancelClicked()
    {
        Navigation.NavigateTo("/Characters");
    }

    private EditCharacterPageState PageState { get; set; } = EditCharacterPageState.MainInformations;

    private void UpdatePageState(EditCharacterPageState newState)
    {
        if (newState != PageState)
        {
            PageState = newState;
            StateHasChanged();
        }
    }

    private static Dictionary<string, EditCharacterPageState> PageStatesDictionary = new() 
    { 
        { "Main", EditCharacterPageState.MainInformations},
        { "Attacks", EditCharacterPageState.Attacks}
    };

    private enum EditCharacterPageState
    {
        MainInformations,
        Attacks
    }


}

