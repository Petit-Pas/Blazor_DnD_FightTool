﻿using DnDEntities.Characters;
using Fight;
using Fight.Characters;
using Microsoft.AspNetCore.Components;
using DnDActions.MartialAttackActions.ExecuteMartialAttack;
using UndoableMediator.Mediators;
using Blazored.Modal.Services;
using Blazored.Modal;
using SharedComponents.Modals.ConfirmationModals;

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
        FightContext.MovingCharacterChanged += FightContext_MovingCharacterChanged;
    }

    private void FightContext_MovingCharacterChanged(object? sender, FightingCharacter? e)
    {
        ResetMovingCharacter();
    }

    private void ResetMovingCharacter()
    {
        MovingCharacter = FightContext.GetMovingCharacter();
        StateHasChanged();
    }

    public async Task Attack()
    {
        var character = FightContext.GetMovingCharacter();
        if (character != null)
        {
            var attackCommand = new ExecuteMartialAttackCommand(character.Id, character.MartialAttacks.First().Id);
            await Mediator.Execute(attackCommand);
        }
    }

}
