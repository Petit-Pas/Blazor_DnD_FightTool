using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Statuses;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

public class MartialAttackTemplateCollection : List<MartialAttackTemplate>
{
    public MartialAttackTemplateCollection() : this(false) 
    { 
    }

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
                    {
                        IsAppliedAutomatically = false,
                        Save = new SaveRollTemplate()
                        {
                            Difficulty = new DifficultyClass(),
                            TargetAbility = AbilityEnum.Wisdom
                        }
                    }
                }
            });
        }
    }

    public MartialAttackTemplate? GetTemplateById(Guid attackId)
    {
        return this.FirstOrDefault(x => x.Id == attackId);
    }
}
