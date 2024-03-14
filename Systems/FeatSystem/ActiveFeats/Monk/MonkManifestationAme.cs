using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkManifestationAme(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ManifestationAmeEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationAmeEffectTag);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation de l'âme - Inactif", StringUtils.gold);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkManifestationAmeEffect(caster.GetAbilityModifier(Ability.Wisdom)));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
        caster.LoginPlayer?.SendServerMessage("Manifestation de l'âme - Actif", StringUtils.gold);

        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationCorpsEffectTag);
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ManifestationEspritEffectTag);
      }
    }
  }
}
