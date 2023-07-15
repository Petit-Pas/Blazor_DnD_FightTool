namespace DnDFightTool.Domain.DnDEntities.HitPoint;

public class HitPoints
{
    /// <summary>
    ///     Should only be used by serialization
    /// </summary>
    public HitPoints() : this(false)
    {
    }

    public HitPoints(bool withDefault)
    {
        MaxHps = 10;
        CurrentHps = 10;
    }

    public int MaxHps { get; set; }

    public int CurrentHps { get; set; }

    public int CurrentTempHps { get; set; }

    public string HpRatioString => $"{CurrentHps}{(CurrentTempHps != 0 ? $"(+ {CurrentHps})" : "")} / {MaxHps}";
}