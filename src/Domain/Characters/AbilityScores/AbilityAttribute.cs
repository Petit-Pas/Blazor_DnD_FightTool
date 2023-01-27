namespace Characters.AbilityScores;

public class AbilityAttribute : Attribute
{
    private AbilityEnum Ability;
    
    public AbilityEnum GetAbility() => Ability;

    public AbilityAttribute(AbilityEnum ability)
    {
        Ability = ability;
    }
}
