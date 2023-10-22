using System.Linq;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void InitThreatRange(NwCreature creature)
    {
      if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.threatAoE.Tag))
        return;

      creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.threatAoE);
    }
  }
}
