using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.DomainExtensions.MartialAttacks;
using FightBlazorComponents.Entities.MartialAttacks;
using MudBlazor;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.MartialAttackQueries.MartialAttackRollResultQueries
{
    public class MartialAttackRollResultQueryHandler : QueryHandlerBase<MartialAttackRollResultQuery, MartialAttackRollResult>
    {
        private readonly IDialogService _dialogServiceProvider;
        private readonly IFightContext _fightContext;

        public MartialAttackRollResultQueryHandler(IFightContext fightContext, IDialogService dialogServiceProvider)
        {
            _dialogServiceProvider = dialogServiceProvider;
            _fightContext = fightContext;
        }

        public override async Task<IQueryResponse<MartialAttackRollResult>> Execute(MartialAttackRollResultQuery query)
        {
            var caster = _fightContext[query.CasterId] ?? throw new NullReferenceException($"{typeof(MartialAttackRollResultQueryHandler)} could not find caster with id {query.CasterId}");
            var attackTemplate = caster.MartialAttacks.GetTemplateByIdOrDefault(query.MartialAttackTemplateId) ?? throw new NullReferenceException($"{typeof(MartialAttackRollResultQueryHandler)} could not find martial attack template with id {query.MartialAttackTemplateId} for caster with id {query.CasterId}");

            var rollableResult = attackTemplate.GetRollableResult();

            var options = new DialogOptions { BackdropClick = false, CloseButton = true };
            var parameters = new DialogParameters<MartialAttackRollResultQueryDialog>
            {
                { x => x.MartialAttackRollResult, rollableResult }
            };

            // TODO this is not working atm
            var dialog = await _dialogServiceProvider.ShowAsync<MartialAttackRollResultQueryDialog>($"{caster.Name} uses {attackTemplate.Name}", parameters, options);
            var result = await dialog.Result;

            if (result?.Canceled ?? true)
            {
                return QueryResponse<MartialAttackRollResult>.Canceled(null!);
            }

            return QueryResponse<MartialAttackRollResult>.Success(rollableResult);
        }
    }
}
