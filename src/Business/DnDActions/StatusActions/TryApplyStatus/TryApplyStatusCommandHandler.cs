using DnDFightTool.Business.DnDActions.LogActions;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.Domain.Fight.DomainExtensions.Statuses;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Infrastructure.Memory.Hashes;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;

public class TryApplyStatusCommandHandler : CommandHandlerBase<TryApplyStatusCommand>
{
    private readonly IFightContext _fightContext;

    public TryApplyStatusCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(TryApplyStatusCommand command)
    {
        var caster = _fightContext[command.CasterId] ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find caster with id {command.CasterId}");
        var target = _fightContext[command.TargetId] ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find target with id {command.TargetId}");
        
        // TODO should warn in the console and stop
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find status to apply with id {command.StatusId}");

        command.StatusHash = status.Hash();

        var saveQueryResult = await QuerySaveRoll(command, status);
        if (saveQueryResult != RequestStatus.Success)
        {
            return new CommandResponse(saveQueryResult);
        }

        if (command.SaveRollResult != null)
        {
            await LogSaveRoll(command.SaveRollResult, caster, target, command);
        }

        await TryApplyStatus(command, status, caster, target);

        return CommandResponse.Success();
    }

    private async Task TryApplyStatus(TryApplyStatusCommand command, StatusTemplate status, FightingCharacter caster, FightingCharacter target)
    {
        if (status.ShouldBeApplied(caster, target, command.SaveRollResult))
        {
            await _mediator.SendAsSubCommandAsync(new ApplyStatusCommand(caster.Id, target.Id, status.Id, command.SaveRollResult), parentCommand: command);
        }
    }

    private async Task LogSaveRoll(SaveRollResult save, FightingCharacter caster, FightingCharacter target, TryApplyStatusCommand command)
    {
        var dc = save.Target.GetValue(caster);
        var modifier = target.AbilityScores.GetSavingModifier(save.Ability);
        var totalSave = save.Result + modifier.Modifier;
        var successful = save.IsSuccessful(caster, target);
        var sign = modifier.Modifier >= 0 ? "+" : "";
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[hover:d20 = {save.Result}, {save.Ability} modifier = {sign}{modifier.Modifier}][b]{totalSave}[/b][/hover] {save.Ability} save (DC {dc}) => [b]{(successful ? "Success" : "Failure")}[/b]"),
            parentCommand: command);
    }

    private async Task<RequestStatus> QuerySaveRoll(TryApplyStatusCommand command, StatusTemplate status)
    {
        if (status.IsAppliedAutomatically)
        {
            return RequestStatus.Success;
        }

        var saveQuery = new SaveRollResultQuery(command.CasterId, command.TargetId, status.Save);
        var saveQueryResponse = await _mediator.QueryAsync(saveQuery);
        command.SaveRollResult = saveQueryResponse.Response;
        return saveQueryResponse.Status;
    }

    public async override Task RedoAsync(TryApplyStatusCommand command)
    {
        var caster = _fightContext[command.CasterId] ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find caster with id {command.CasterId}");
        var target = _fightContext[command.TargetId] ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find target with id {command.TargetId}");
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new NullReferenceException($"{typeof(TryApplyStatusCommandHandler)} could not find status to apply with id {command.StatusId}");

        var statusHash = status.Hash();
        if (statusHash != command.StatusHash)
        {
            command.SaveRollResult = null;
            command.StatusHash = statusHash;
            
            // Cannot accept a failure here, since we are in a redo operation
            var result = await QuerySaveRoll(command, status);
            while (result != RequestStatus.Success)
            {
                result = await QuerySaveRoll(command, status);
            }
        }

        ClearSubCommands(command);

        await TryApplyStatus(command, status, caster, target);
    }
}
