﻿using Blazored.Modal;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.SaveQueries;

public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IModalServiceProvider _modalServiceProvider;
    private readonly IFightContext _fightContext;

    public SaveRollResultQueryHandler(IFightContext fightContext, IModalServiceProvider modalServiceProvider)
    {
        _modalServiceProvider = modalServiceProvider;
        _fightContext = fightContext;
    }

    public async override Task<IQueryResponse<SaveRollResult>> Execute(SaveRollResultQuery query)
    {
        var modalService = _modalServiceProvider.GetModalService();
        ArgumentNullException.ThrowIfNull(modalService, nameof(modalService));

        var caster = _fightContext.GetCharacterById(query.CasterId);
        var target = _fightContext.GetCharacterById(query.TargetId);
        var saveRollResult = query.Save.GetEmptyRollResult();

        if (caster == null || target == null)
        {
            // TODO should warn in the console and stop
#pragma warning disable
            throw new ArgumentNullException("Could not find caster or target.");
#pragma warning restore
        }

        var parameters = new ModalParameters()
            .Add(nameof(SaveRollResultQueryModal.SaveRollResult), saveRollResult)
            .Add(nameof(SaveRollResultQueryModal.Caster), caster).
            Add(nameof(SaveRollResultQueryModal.Target), target);
        var options = new ModalOptions() { UseCustomLayout = true };
        var modal = modalService.Show<SaveRollResultQueryModal>("Saving throw", parameters, options);

        var result = await modal.Result;

        if (result.Cancelled)
        {
            return QueryResponse<SaveRollResult>.Canceled(null!);
        }

        return QueryResponse<SaveRollResult>.Success(saveRollResult);
    }
}
