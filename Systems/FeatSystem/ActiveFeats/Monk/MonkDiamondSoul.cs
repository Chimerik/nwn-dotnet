using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DiamondSoul(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.DiamondSoulEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.DiamondSoulEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        caster.LoginPlayer?.SendServerMessage("Âme de diamant - Inactif", StringUtils.gold);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DiamondSoul);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
        caster.LoginPlayer?.SendServerMessage("Âme de diamant - Actif", StringUtils.gold);
      }
    }
  }
}
