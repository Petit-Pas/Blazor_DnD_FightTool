using Blazored.Toast.Services;
using DnDFightTool.Domain.DnDEntities.Characters;
using Mapping;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Pages;

public partial class EditCharacterPage
{
    [Parameter]
    public required string CharacterId { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }
    [Inject]
    public required IToastService ToastService { get; set; }
    [Inject]
    public required IMapper Mapper { get; set; }


    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

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
            var character = CharacterRepository.GetCharacterById(id);
            if (character == null)
            {
                Navigation.NavigateTo("/oldCharacters");
                return;
            }
            _character = Mapper.Copy(CharacterRepository.GetCharacterById(id)!);
        }
    }

    private void SaveClicked()
    {
        CharacterRepository.Save(_character!);
        ToastService.ShowError($"Character {_character?.Name} has been successfully saved!");
        Navigation.NavigateTo("/oldCharacters");
    }

    private void CancelClicked()
    {
        Navigation.NavigateTo("/oldCharacters");
    }

    private EditCharacterPageState PageState { get; set; } = EditCharacterPageState.MainInformations;

    private readonly static Dictionary<string, EditCharacterPageState> _pageStatesDictionary = new()
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

