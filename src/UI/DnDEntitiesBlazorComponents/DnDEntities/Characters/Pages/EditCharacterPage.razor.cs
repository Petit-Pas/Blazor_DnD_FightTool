using DnDEntities.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Pages;

public partial class EditCharacterPage
{
    [Parameter]
    public string CharacterId { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }


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
            _character = CharacterRepository.GetCharacterById(id);
        }
    }

    private void SaveClicked()
    {
        CharacterRepository.Save(_character!);
        Navigation.NavigateTo("/Characters");
    }

    private void CancelClicked()
    {
        Navigation.NavigateTo("/Characters");
    }
}