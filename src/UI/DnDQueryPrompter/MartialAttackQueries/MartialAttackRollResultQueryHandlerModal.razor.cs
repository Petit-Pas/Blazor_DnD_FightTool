using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDQueryPrompter.MartialAttackQueries;

/// <summary>
///     Modal dialog for entering the result of a martial attack roll.
///     The user selects a target, then fills in the hit roll and damage rolls.
/// </summary>
public partial class MartialAttackRollResultQueryHandlerModal
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    /// <summary>
    ///     The id of the character performing the attack.
    /// </summary>
    [Parameter]
    public Guid CasterId { get; set; }

    /// <summary>
    ///     The id of the martial attack template being used.
    /// </summary>
    [Parameter]
    public Guid AttackId { get; set; }

    private MartialAttackRollResult? _rollResult;
    private Guid? _selectedTargetId;
    private Guid[] _excludedFighterIds = [];
    private MartialAttackTemplate? _attackTemplate;
    private ICharacter? _caster;

    internal bool CanValidate => _selectedTargetId is not null
        && _rollResult is not null
        && _rollResult.HitRoll.Result > 0;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _excludedFighterIds = [CasterId];

        var caster = FightContext[CasterId];
        if (caster is null)
        {
            return;
        }

        _caster = caster;

        _attackTemplate = caster.MartialAttacks.GetTemplateByIdOrDefault(AttackId);
        if (_attackTemplate is null)
        {
            return;
        }

        var hitRoll = new HitRollResult(_attackTemplate.ToHitModifiers);
        var damageRolls = _attackTemplate.Damages.Select(d => d.GetEmptyRollResult()).ToArray();
        _rollResult = new MartialAttackRollResult(hitRoll, damageRolls);
    }

    private Task ConfirmAsync()
    {
        if (_rollResult is null || _selectedTargetId is null)
        {
            return Task.CompletedTask;
        }

        _rollResult.TargetId = _selectedTargetId.Value;
        MudDialog.Close(DialogResult.Ok(_rollResult));

        return Task.CompletedTask;
    }

    private IEnumerable<FightingCharacter> AvailableTargets =>
        FightContext.Fighters.Where(f => !_excludedFighterIds.Contains(f.Id));

    private FightingCharacter? SelectedTarget
    {
        get => _selectedTargetId is not null ? FightContext[_selectedTargetId.Value] : null;
        set => _selectedTargetId = value?.Id;
    }

    internal int GetDamageTotal(DamageRollResult damageRoll)
    {
        if (_caster is null)
        {
            return damageRoll.Damage;
        }

        var modifier = damageRoll.Dices.GetScoreModifier(_caster);
        return damageRoll.Damage + modifier.Modifier;
    }
}
