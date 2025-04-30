using Blazored.Modal;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.MartialAttackQueries
{
    public class MartialAttackRollResultQueryHandler : QueryHandlerBase<MartialAttackRollResultQuery, MartialAttackRollResult>
    {
        private readonly IModalServiceProvider _modalServiceProvider;
        private readonly IFightContext _fightContext;

        public MartialAttackRollResultQueryHandler(IFightContext fightContext, IModalServiceProvider modalServiceProvider)
        {
            _modalServiceProvider = modalServiceProvider;
            _fightContext = fightContext;
        }

        public async override Task<IQueryResponse<MartialAttackRollResult>> Execute(MartialAttackRollResultQuery query)
        {
            var modalService = _modalServiceProvider.GetModalService();
            ArgumentNullException.ThrowIfNull(modalService, nameof(modalService));

            var caster = query.GetCaster(_fightContext);
            var attackTemplate = caster.MartialAttacks.GetTemplateByIdOrDefault(query.MartialAttackTemplateId) ?? throw new InvalidOperationException($"Could not get attack template.");

            var damageRolls = attackTemplate.Damages.Select(x => x.GetEmptyRollResult());
            var martialAttackRollResult = new MartialAttackRollResult(new HitRollResult(attackTemplate.Modifiers), [.. damageRolls]);


            var parameters = new ModalParameters()
                .Add(nameof(MartialAttackRollResultQueryModal.MartialAttackRollResult), martialAttackRollResult)
                .Add(nameof(MartialAttackRollResultQueryModal.Caster), caster);
            var options = new ModalOptions() { UseCustomLayout = true };
            var modal = modalService.Show<MartialAttackRollResultQueryModal>("title", parameters, options);

            var result = await modal.Result;

            if (result.Cancelled)
            {
                return QueryResponse<MartialAttackRollResult>.Canceled(null!);
            }

            return QueryResponse<MartialAttackRollResult>.Success(martialAttackRollResult);
        }
    }
}
