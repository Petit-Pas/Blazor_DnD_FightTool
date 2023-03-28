namespace Fight.AttackRolls.ArmorClasses;

public class ArmorClass
{
    public int BaseArmorClass { get; set; } = 10;

    public int ShieldArmorClass { get; set; } = 2;

    public bool HasShieldEquipped { get; set; } = false;

    public int EffectiveAC => BaseArmorClass + (HasShieldEquipped ? ShieldArmorClass : 0);
}