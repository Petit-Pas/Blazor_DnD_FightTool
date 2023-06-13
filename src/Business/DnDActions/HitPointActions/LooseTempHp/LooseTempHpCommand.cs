using Fight.Characters;

namespace DnDActions.HitPointActions.LooseTempHp;

public class LooseTempHpCommand : HitPointCommandBase
{
    /// <summary>
    ///     This is the expected amount of temp hp lost, could be lowered in reality if the amount of temp Hp is lower
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     This is the actual amount of temp HP that were reduced
    /// </summary>
    public int? CorrectedAmount { get; set; }

    public LooseTempHpCommand(FightingCharacter fightingCharacter, int amount) : base(fightingCharacter.CharacterId)
	{
		Amount = amount;
	}
}
