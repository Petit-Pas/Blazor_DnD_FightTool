using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.FightActions.AddToFight;

/// <summary>
///     Atomic handler for <see cref="AddToFightAtomicCommand"/>.
///     Resolves the character, adds it to the fight, sets the initiative roll, and records the new fighter id for undo.
///     Returns the added fighter's <see cref="Guid"/> id.
/// </summary>
public class AddToFightAtomicCommandHandler : CommandHandlerBase<AddToFightAtomicCommand, Guid>
{
    private readonly IFightContext _fightContext;
    private readonly ICharacterRepository _characterRepository;

    /// <summary>
    ///     Ctor.
    /// </summary>
    public AddToFightAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext, ICharacterRepository characterRepository) : base(mediator)
    {
        _fightContext = fightContext;
        _characterRepository = characterRepository;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<Guid>> ExecuteAsync(AddToFightAtomicCommand command)
    {
        var character = _characterRepository.GetCharacterById(command.SourceCharacterId)
            ?? throw new ArgumentException($"{nameof(AddToFightAtomicCommandHandler)} could not find character with id {command.SourceCharacterId}");

        var addedFighter = _fightContext.Add(character, command.Initiative);

        if (addedFighter is null)
        {
            return Task.FromResult<ICommandResponse<Guid>>(CommandResponse.Failed<Guid>());
        }

        command.AddedFighterId = addedFighter.Id;

        return Task.FromResult<ICommandResponse<Guid>>(CommandResponse.Success<Guid>(addedFighter.Id));
    }

    /// <inheritdoc />
    public override Task UndoAsync(AddToFightAtomicCommand command)
    {
        if (command.AddedFighterId is null)
        {
            throw new InvalidOperationException($"Cannot undo {nameof(AddToFightAtomicCommand)}: {nameof(command.AddedFighterId)} is null.");
        }

        _fightContext.Remove(command.AddedFighterId.Value);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task RedoAsync(AddToFightAtomicCommand command)
    {
        if (command.AddedFighterId is null)
        {
            throw new InvalidOperationException($"Cannot redo {nameof(AddToFightAtomicCommand)}: {nameof(command.AddedFighterId)} is null.");
        }

        _fightContext.Restore(command.AddedFighterId.Value);
        return Task.CompletedTask;
    }
}
