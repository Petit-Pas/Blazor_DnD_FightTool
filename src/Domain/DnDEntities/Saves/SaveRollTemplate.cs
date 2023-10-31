﻿using DnDFightTool.Domain.DnDEntities.AbilityScores;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Saves;

public class SaveRollTemplate : IHashable
{
    /// <summary>
    ///     The ability of the target to use for the check.
    /// </summary>
    public AbilityEnum TargetAbility { get; set; } = AbilityEnum.Wisdom;

    /// <summary>
    ///     Set to the DC of the caster by default
    /// </summary>
    public DifficultyClass Difficulty { get; set; } = new DifficultyClass("DC");

    public SaveRollResult GetEmptyRollResult()
    {
        return new SaveRollResult(Difficulty, TargetAbility);
    }
}
