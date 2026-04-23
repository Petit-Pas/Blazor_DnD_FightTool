using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Domain.CharacterSheet.Statuses;

namespace DnDFightTool.Domain.CharacterSheet.MartialAttacks;

/// <summary>
///     Collection of <see cref="MartialAttackTemplate" />
/// </summary>
public class MartialAttackTemplateCollection : Dictionary<Guid, MartialAttackTemplate>
{
    /// <summary>
    ///     Empty ctor, should only be used by serializers
    /// </summary>
    [Obsolete("Should only be used by serializers")]
    public MartialAttackTemplateCollection() : this(false) 
    { 
    }

    /// <summary>
    ///     Ctor with tha ability to add a default attack.   
    /// </summary>
    /// <param name="withDefault"></param>
    public MartialAttackTemplateCollection(bool withDefault = false)
    {
        if (withDefault)
        {
            Add(new MartialAttackTemplate()
            {
                Damages =
                [
                    new DamageRollTemplate()
                    {
                        Dices = new DiceRollTemplate("2d12"),
                        Type = DamageTypeEnum.Cold
                    },
                    new DamageRollTemplate()
                    {
                        Dices = new DiceRollTemplate("1d4+3"),
                        Type = DamageTypeEnum.Thunder
                    }
                ],
                Statuses = new StatusTemplateCollection()
                {
                    new StatusTemplate()
                }
            });
        }
    }

    public void Add(MartialAttackTemplate attackTemplate)
    {
        this[attackTemplate.Id] = attackTemplate;
    }

    /// <summary>
    ///     Get an attack template by its id if it exists
    /// </summary>
    /// <param name="attackId"></param>
    /// <returns></returns>
    public MartialAttackTemplate? GetTemplateByIdOrDefault(Guid attackId)
    {
        TryGetValue(attackId, out var attackTemplate);
        return attackTemplate;
    }
}
