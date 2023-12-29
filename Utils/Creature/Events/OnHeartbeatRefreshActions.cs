using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnHeartbeatRefreshActions(ModuleEvents.OnHeartbeat onHB)
    {
      foreach (var creature in NwObject.FindObjectsOfType<NwCreature>())
      {
        creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value = 1;
        creature.GetObjectVariable<LocalVariableInt>(HastMasterCooldownVariable).Delete();

        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
          continue;

        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
      }
    }
  }
}
