using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnCombatStartRefreshActions(OnCombatRoundStart onStartCombatRound)
    {
       onStartCombatRound.Creature.GetObjectVariable<LocalVariableInt>("_BONUS_ACTION").Value = 1;

      if (onStartCombatRound.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
        return;

      onStartCombatRound.Creature.GetObjectVariable<LocalVariableInt>("_REACTION").Value = 1;
    }
  }
}
