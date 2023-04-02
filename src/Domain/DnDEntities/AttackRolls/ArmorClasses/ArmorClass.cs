using System.Text.Json.Serialization;

namespace DnDEntities.AttackRolls.ArmorClasses;

public class ArmorClass
{
    public static readonly ArmorClass Default = new ArmorClass();

    public int BaseArmorClass { get; set; } = 10;

    public int ShieldArmorClass { get; set; } = 2;

    public bool HasShieldEquipped { get; set; } = false;

    [JsonIgnore]
    public int EffectiveAC => BaseArmorClass + (HasShieldEquipped ? ShieldArmorClass : 0);
}