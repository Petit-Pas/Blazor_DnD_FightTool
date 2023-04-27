using DnDEntities.Characters;
using System.Collections;
using Fight.Characters;

namespace Fight;

public class FightContext : IFightContext
{
    private readonly List<FightingCharacter> _fighters = new ();

    public void AddToFight(Character character)
    {
        _fighters.Add(new FightingCharacter(character));
    }

    public void SetFightingCharacter(FightingCharacter fightingCharacter)
    {
        if (!_fighters.Contains(fightingCharacter))
        {
            // TODO Warn
            return;
        }

        if (MovingCharacter != fightingCharacter)
        {
            MovingCharacter = fightingCharacter;
            RaiseMovingCharacterChanged();
        }
    }


    public FightingCharacter? MovingCharacter { get; private set; }

    public event EventHandler<FightingCharacter?>? MovingCharacterChanged;

    private void RaiseMovingCharacterChanged()
    {
        MovingCharacterChanged?.Invoke(this, MovingCharacter);
    }

    public IEnumerator<FightingCharacter> GetEnumerator()
    {
        return _fighters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}