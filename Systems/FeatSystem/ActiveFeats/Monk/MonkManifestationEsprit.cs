using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkManifestationEsprit(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ManifestationEspritEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationEspritEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation de l'esprit - Inactif", StringUtils.gold);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkManifestationEspritEffect(caster.GetAbilityModifier(Ability.Wisdom)));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation de l'esprit - Actif", StringUtils.gold);

        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationCorpsEffectTag);
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationAmeEffectTag);
      }
    }
  }
}
