using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnRecoveryAddThreatRange(NwCreature target, Effect effect)
    {
      if (!EffectUtils.IsIncapacitatingEffect(effect)
        || target.ActiveEffects.Any(e => EffectUtils.IsIncapacitatingEffect(e)))
        return;

      CreatureUtils.InitThreatRange(target);
    }
  }
}
