using Fight.Characters;

namespace DnDActions.HitPointActions.LooseHp;

public class LooseHpCommand : HitPointCommandBase
{
    /// <summary>
    ///     This is the expected amount of hp removed, could be lowered in reality if the amount of Hp is not sufficient
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     This is the actual amount of HP that were removed
    /// </summary>
    public int? CorrectedAmount { get; set; }

    public LooseHpCommand(FightingCharacter fightingCharacter, int amount) : base(fightingCharacter.CharacterId)
    {
        Amount = amount;
    }
}
