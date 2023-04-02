namespace DnDEntities.AbilityScores;

public class AbilityAttribute : Attribute
{
    private readonly AbilityEnum _ability;
    
    public AbilityEnum GetAbility() => _ability;

    public AbilityAttribute(AbilityEnum ability)
    {
        _ability = ability;
    }
}
