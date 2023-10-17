using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnCombatStartRefreshActions(OnCombatRoundStart onStartCombatRound)
    {
      onStartCombatRound.Creature.GetObjectVariable<LocalVariableInt>("_REACTION").Value = 1;
      onStartCombatRound.Creature.GetObjectVariable<LocalVariableInt>("_BONUS_ACTION").Value = 1;
    }
  }
}
