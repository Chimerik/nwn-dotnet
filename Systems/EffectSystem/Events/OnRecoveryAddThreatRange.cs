using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnRecoveryAddThreatRange(OnEffectRemove onEffect)
    {
      if (!EffectUtils.IsCannotThreatenEffect(onEffect.Effect.EffectType) || onEffect.Object is not NwCreature creature 
        || creature.ActiveEffects.Any(e => EffectUtils.IsCannotThreatenEffect(e.EffectType)))
        return;

      CreatureUtils.InitThreatRange(creature);
    }
  }
}
