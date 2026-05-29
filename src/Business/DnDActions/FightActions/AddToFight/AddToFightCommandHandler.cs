using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDQueries.FightQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Queries;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.FightActions.AddToFight;

/// <summary>
///     Orchestrator: resolves initiative (prompt or inherit), dispatches <see cref="AddToFightAtomicCommand"/>
///     (mutation) then logs the added fighter.
///     Cancel-before-mutation: a cancelled prompt aborts with <see cref="RequestStatus.Canceled"/> and no state change.
///     Undo cascades to sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class AddToFightCommandHandler : CommandHandlerBase<AddToFightCommand>
{
    private readonly IFightContext _fightContext;
    private readonly ICharacterRepository _characterRepository;

    /// <summary>
    ///     Ctor.
    /// </summary>
    public AddToFightCommandHandler(
        IUndoableMediator mediator,
        IFightContext fightContext,
        ICharacterRepository characterRepository) : base(mediator)
    {
        _fightContext = fightContext;
        _characterRepository = characterRepository;
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(AddToFightCommand command)
    {
        var character = _characterRepository.GetCharacterById(command.SourceCharacterId);
        if (character is null)
        {
            return new CommandResponse(RequestStatus.Failed);
        }

        var initiativeRollResult = await ResolveInitiativeRollAsync(character);
        if (initiativeRollResult.Status != RequestStatus.Success)
        {
            return new CommandResponse(initiativeRollResult.Status);
        }

        var atomicCmd = new AddToFightAtomicCommand(command.SourceCharacterId, initiativeRollResult.Response);
        var atomicResponse = await _mediator.SendAsSubCommandAsync(atomicCmd, parentCommand: command);
        if (atomicResponse.Status != RequestStatus.Success)
        {
            return new CommandResponse(atomicResponse.Status);
        }

        command.AddedFighterId = atomicResponse.Response;
        command.InitiativeRoll = initiativeRollResult.Response;

        await LogAddedFighter(character.Name, initiativeRollResult.Response, command);

        return CommandResponse.Success();
    }

    /// <inheritdoc />
    public override Task UndoAsync(AddToFightCommand command)
    {
        return base.UndoAsync(command);
    }

    private async Task<IQueryResponse<int>> PromptInitiativeRoll(Guid characterId)
    {
        return await _mediator.QueryAsync(new InitiativeRollQuery(characterId));
    }

    private async Task<IQueryResponse<int>> ResolveInitiativeRollAsync(ICharacter character)
    {
        int? initiativeRoll = null;

        if (character.Type == CharacterType.Monster)
        {
            var sameKind = _fightContext.Fighters.FirstOrDefault(f => f.OriginalCharacterId == character.Id);
            if (sameKind is not null)
            {
                initiativeRoll = sameKind.InitiativeRoll;
            }
        }

        // If initiative roll not inherited, prompt for it
        if (initiativeRoll is null)
        {
            return await PromptInitiativeRoll(character.Id);
        }

        return QueryResponse<int>.Success(initiativeRoll.Value);
    }

    private async Task LogAddedFighter(string fighterName, int initiative, AddToFightCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] joined the fight (initiative [b]{initiative}[/b])"),
            parentCommand: command);
    }
}

