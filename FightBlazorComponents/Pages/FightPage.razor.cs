using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;
using UndoableMediator.Mediators;
using Blazored.Modal.Services;
using UndoableMediator.Requests;

namespace FightBlazorComponents.Pages;

public partial class FightPage
{
    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required IUndoableMediator Mediator { get; set; }

    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IModalServiceProvider ModalServiceProvider { get; set; }

    [CascadingParameter]
    public required IModalService ModalService {
        set
        {
            if (ModalServiceProvider.GetModalService() == null)
            {
                ModalServiceProvider.ConfigureModalService(value);
            }
        } 
    }/*{ get => SingletonModalService; set => SingletonModalService = value; }*/
    
    // Commands need an access to this instance, and can't inject it since the proper instance is a cascading parameter, so a singleton fixed it.
    //public static IModalService SingletonModalService;

    private FightingCharacter? _movingCharacter = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetMovingCharacter();
        FightContext.ActiveFighterChanged += FightContext_MovingCharacterChanged;
    }

    private void FightContext_MovingCharacterChanged(object? sender, FightingCharacter? e)
    {
        ResetMovingCharacter();
    }

    private void ResetMovingCharacter()
    {
        _movingCharacter = FightContext.ActiveFighter;
        StateHasChanged();
    }

    public async Task Attack()
    {
        var character = FightContext.ActiveFighter;
        if (character != null)
        {
            var attackCommand = new ExecuteMartialAttackCommand(character.Id, character.MartialAttacks.Keys.First());
            await Mediator.Execute(attackCommand, (status) => status is RequestStatus.Success);
        }
    }

    public bool CanUndo => Mediator?.HistoryLength != 0;

    public bool CanRedo => Mediator?.RedoHistoryLength != 0;

    public void Undo()
    {
        if (CanUndo)
        {
            Mediator.UndoLastCommand();
        }
    }

    public async Task Redo()
    {
        if (CanRedo)
        {
            await Mediator.RedoLastUndoneCommand();
        }
    }
}
