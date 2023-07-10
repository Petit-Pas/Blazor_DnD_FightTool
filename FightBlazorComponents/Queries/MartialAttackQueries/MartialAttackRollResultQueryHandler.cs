using System.Reflection;
using Blazored.Modal;
using Blazored.Modal.Services;
using DnDEntities.Dices.DiceThrows;
using DnDQueries.MartialAttackQueries;
using Fight;
using Fight.MartialAttacks;
using FightBlazorComponents.Pages;
using SharedComponents.Modals.ConfirmationModals;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.MartialAttackQueries
{
    public class MartialAttackRollResultQueryHandler : QueryHandlerBase<MartialAttackRollResultQuery, MartialAttackRollResult>
    {
        private readonly IModalService _modalService;
        private readonly IFightContext _fightContext;

        public MartialAttackRollResultQueryHandler(IFightContext fightContext)
        {
            _modalService = FightPage.SingletonModalService;
            _fightContext = fightContext;
        }

        public override async Task<IQueryResponse<MartialAttackRollResult>> Execute(MartialAttackRollResultQuery query)
        {
            var caster = query.GetCaster(_fightContext);
            var attackTemplate = caster.MartialAttacks.GetTemplateById(query.MartialAttackTemplateId) ?? throw new InvalidOperationException($"Could not get attack template.");

            var damageRolls = attackTemplate.Damages.Select(x => x.GetEmptyRollResult());
            var martialAttackRollResult = new MartialAttackRollResult(new HitRollResult(attackTemplate.Modifiers), damageRolls.ToArray());


            var parameters = new ModalParameters()
                .Add(nameof(MartialAttackRollResultQueryModal.MartialAttackRollResult), martialAttackRollResult)
                .Add(nameof(MartialAttackRollResultQueryModal.Caster), caster);
            var options = new ModalOptions() { UseCustomLayout = true };
            var modal = _modalService.Show<MartialAttackRollResultQueryModal>("title", parameters, options);

            var result = await modal.Result;

            if (result.Cancelled)
            {
                return QueryResponse<MartialAttackRollResult>.Canceled(null!);
            }

            return QueryResponse<MartialAttackRollResult>.Success(martialAttackRollResult);
        }
    }
}
