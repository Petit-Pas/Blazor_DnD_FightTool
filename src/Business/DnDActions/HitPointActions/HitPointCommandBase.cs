using DnDEntities.HitPoint;
using Fight;
using UndoableMediator.Commands;

namespace DnDActions.HitPointActions
{
    public class HitPointCommandBase : CommandBase
    {
        public HitPointCommandBase(Guid characterId)
        {
            CharacterId = characterId;
        }

        /// <summary>
        ///     Guid of the character affected by the command
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        ///     Simple helper method for the command handlers, to get the hitPoints easily
        /// </summary>
        /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
        /// <returns> the affected HitPoints </returns>
        internal HitPoints? GetHitPoints(IFightContext fightContext)
        {
            return fightContext.GetCharacterById(CharacterId)?.HitPoints;
        }
    }
}
