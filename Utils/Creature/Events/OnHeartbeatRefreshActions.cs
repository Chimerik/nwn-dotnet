using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnHeartbeatRefreshActions(CreatureEvents.OnHeartbeat onHB)
    {
      onHB.Creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value = 1;
      onHB.Creature.GetObjectVariable<LocalVariableInt>(HastMasterCooldownVariable).Delete();

      if (onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
        return;

      onHB.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
    }
  }
}
