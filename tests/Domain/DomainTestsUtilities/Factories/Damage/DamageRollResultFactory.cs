﻿using DnDEntities.Damage;
using Fight.Damage;

namespace FightTestsUtilities.Factories.Damage;

public static class DamageRollResultFactory
{
    public static DamageRollResult Build(DamageTypeEnum damageType, int damage)
    {
        return new DamageRollResult()
        {
            Damage = damage,
            DamageType = damageType,
        };
    }
}