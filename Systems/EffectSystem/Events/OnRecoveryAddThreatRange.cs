using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnRecoveryAddThreatRange(NwCreature target, EffectType effectType)
    {
      if (!EffectUtils.IsCannotThreatenEffect(effectType)
        || target.ActiveEffects.Any(e => EffectUtils.IsCannotThreatenEffect(e.EffectType)))
        return;

      CreatureUtils.InitThreatRange(target);
    }
  }
}
