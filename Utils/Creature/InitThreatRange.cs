using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void InitThreatRange(NwCreature creature)
    {
      if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ThreatenedAoETag))
        return;

      creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.threatAoE(creature));

      float threatRadius = (CreaturePlugin.GetHitDistance(creature) / 0.3f) * 3;

      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(threatRadius);
    }
  }
}
