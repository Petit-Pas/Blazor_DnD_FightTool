using DnDEntities.Characters;

namespace Fight;

public class FightContext : IFightContext
{
    private List<Character> Fighters = new ();

    public void AddToFight(Character character)
    {
        Fighters.Add(character);
    }
}