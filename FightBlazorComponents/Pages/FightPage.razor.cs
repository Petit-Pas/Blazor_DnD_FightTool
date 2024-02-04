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
    public IFightContext FightContext { get; set; }

    [Inject]
    public IUndoableMediator Mediator { get; set; }

    [Inject]
    public ICharacterRepository CharacterRepository { get; set; }

    [CascadingParameter]
    public IModalService ModalService { get => SingletonModalService; set => SingletonModalService = value; }
    // Commands need an access to this instance, and can't inject it since the proper instance is a cascading parameter, so a singleton fixed it.
    public static IModalService SingletonModalService;

    private Character? MovingCharacter = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetMovingCharacter();
        FightContext.MovingFighterChanged += FightContext_MovingCharacterChanged;
    }

    private void FightContext_MovingCharacterChanged(object? sender, Fighter? e)
    {
        ResetMovingCharacter();
    }

    private void ResetMovingCharacter()
    {
        MovingCharacter = FightContext.GetMovingFighterCharacter();
        StateHasChanged();
    }

    public async Task Attack()
    {
        var character = FightContext.GetMovingFighterCharacter();
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
