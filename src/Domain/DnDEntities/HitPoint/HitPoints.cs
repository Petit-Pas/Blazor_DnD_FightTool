namespace DnDFightTool.Domain.DnDEntities.HitPoint;

/// <summary>
///     Represents the hit points of a character
/// </summary>
public class HitPoints
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public HitPoints() 
    {
    }

    /// <summary>
    ///     The maximum hit points of a character
    /// </summary>
    public int MaxHps { get; set; } = 10;

    /// <summary>
    ///     The current hit points of a character
    /// </summary>
    public int CurrentHps { get; set; } = 10;

    /// <summary>
    ///     The current temporary hit points of a character
    ///     When receiving temporary hit points, the character should get the max between the current temporary hit points and the new temporary hit points
    /// </summary>
    public int CurrentTempHps { get; set; } = 0;

    /// <summary>
    ///     A fraction like string to display the current hit points of a character as text.
    /// </summary>
    public string HpRatioString => $"{CurrentHps}{(CurrentTempHps != 0 ? $"(+ {CurrentHps})" : "")} / {MaxHps}";
}