namespace DnDFightTool.Domain.DnDEntities.Skills;

/// <summary>
///    Enum for skills of D&D
/// </summary>
public enum SkillMasteryEnum
{
    // TODO Maybe I could change DamageFactor to a simple Factor, so that I can use it in here too.
    // If I do this, I should add an unknown here too that return a factor of 0 

    /// <summary>
    ///    Character is not proficient with this skill in any way.
    /// </summary>
    Normal,
    /// <summary>
    ///     Character is a master with this skill, any check will add mastery bonus.
    /// </summary>
    Mastery,
    /// <summary>
    ///     Character is an expert with this skill, any check will add mastery bonus twice.
    /// </summary>
    Expertise
}