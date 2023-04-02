using DnDEntities.Characters;

namespace Fight;

public interface IFightContext
{
    public void AddToFight(Character character);
}