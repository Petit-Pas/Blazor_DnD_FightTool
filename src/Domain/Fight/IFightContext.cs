using DnDEntities.Characters;

namespace Fight;

public interface IFightContext: IEnumerable<Character>
{
    public void AddToFight(Character character);

    public Character? this[Guid characterId] { get; }
}