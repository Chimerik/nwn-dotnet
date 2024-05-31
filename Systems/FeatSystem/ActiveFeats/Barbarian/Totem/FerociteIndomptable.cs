using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FerociteIndomptable(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
        caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(NwRandom.Roll(Utils.random, 8) + caster.GetAbilityModifier(Ability.Constitution)));
      else
        caster.LoginPlayer?.SendServerMessage("Utilisable uniquement sous les effets de Rage du Barbare", ColorConstants.Red);

      caster.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 0);
    }
  }
}
