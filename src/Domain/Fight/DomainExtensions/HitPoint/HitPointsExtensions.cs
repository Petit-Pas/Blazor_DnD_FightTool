using DnDFightTool.Domain.CharacterSheet.HitPoint;

namespace DnDFightTool.Domain.Fight.DomainExtensions.HitPoint;

/// <summary>
///     Extension class for <see cref="HitPoints" />
/// </summary>
public static class HitPointsExtensions
{
    /// <summary>
    ///     A ratio from 0 to 100 representing the percentage health of a character.
    /// </summary>
    /// <param name="hitPoints"></param>
    /// <returns></returns>
    public static double GetHealthRatio(this HitPoints hitPoints)
    {
        ArgumentNullException.ThrowIfNull(hitPoints);

        return (double)hitPoints.CurrentHps / hitPoints.MaxHps * 100;
    }
}
