using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks
{
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
                });
            }
        }

        public MartialAttackTemplate? GetTemplateById(Guid attackId)
        {
            return this.FirstOrDefault(x => x.Id == attackId);
        }
    }
}
