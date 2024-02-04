using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Statuses;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

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
            this.Add(new MartialAttackTemplate()
            {
                Damages = new DamageRollTemplateCollection()
                {
                    new DamageRollTemplate()
                    {
                        Dices = new DiceThrowTemplate("2d12"),
                        Type = DamageTypeEnum.Cold
                    },
                    new DamageRollTemplate()
                    {
                        Dices = new DiceThrowTemplate("1d4+3"),
                        Type = DamageTypeEnum.Thunder
                    }
                },
                Statuses = new StatusTemplateCollection()
                {
                    new StatusTemplate()
                }
            });
        }
    }

    public void Add(MartialAttackTemplate attackTemplate)
    {
        Add(attackTemplate.Id, attackTemplate);
    }

    /// <summary>
    ///     Get an attack template by its id if it exists
    /// </summary>
    /// <param name="attackId"></param>
    /// <returns></returns>
    public MartialAttackTemplate? GetTemplateByIdOrDefault(Guid attackId)
    {
        this.TryGetValue(attackId, out var attackTemplate);
        return attackTemplate;
    }
}
