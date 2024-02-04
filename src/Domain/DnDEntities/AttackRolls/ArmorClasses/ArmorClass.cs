using System.Text.Json.Serialization;

namespace DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;

/// <summary>
///    Armor class of a character
/// </summary>
public class ArmorClass
{
    /// <summary>
    ///     The base armor class of the character (no computation here, if he has a magic +1 14+2 studded armor, just set it to 17)
    /// </summary>
    public int BaseArmorClass { get; set; } = 10;

    /// <summary>
    ///    The armor class provided by the shield if equipped
    /// </summary>
    public int ShieldArmorClass { get; set; } = 2;

    /// <summary>
    ///    If the character has a (its) shield equipped
    /// </summary>
    public bool HasShieldEquipped { get; set; } = false;

    /// <summary>
    ///     A property to help compute the actual AC of the character (with shield if equipped)
    /// </summary>
    [JsonIgnore]
    public int EffectiveAC => BaseArmorClass + (HasShieldEquipped ? ShieldArmorClass : 0);
}