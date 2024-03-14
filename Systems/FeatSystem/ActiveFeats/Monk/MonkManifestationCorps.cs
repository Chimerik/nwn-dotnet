using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkManifestationCorps(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ManifestationCorpsEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationCorpsEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation du corps - Inactif", StringUtils.gold);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkManifestationCorpsEffect(caster.GetAbilityModifier(Ability.Wisdom)));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation du corps - Actif", StringUtils.gold);

        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationAmeEffectTag);
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationEspritEffectTag);
      }
    }
  }
}
