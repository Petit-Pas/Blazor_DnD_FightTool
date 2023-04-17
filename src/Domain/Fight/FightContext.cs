using DnDEntities.Characters;
using System.Collections;

namespace Fight;

public class FightContext : IFightContext
{
    private readonly Dictionary<Guid, Character> Fighters = new ();

    public Character? this[Guid characterId] 
    { 
        get
        {
            if (Fighters.TryGetValue(characterId, out var character))
            {
                return character;
            }
            return null;
        }
    }

    public void AddToFight(Character character)
    {
        Fighters.Add(character.Id, character);
    }

    public IEnumerator<Character> GetEnumerator()
    {
        return Fighters.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}