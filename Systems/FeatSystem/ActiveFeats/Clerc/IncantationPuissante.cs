using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void IncantationPuissante(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }
      int wisMod = caster.GetAbilityModifier(Ability.Wisdom) > 0 ? caster.GetAbilityModifier(Ability.Wisdom) : 1;

      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(wisMod * 2));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercIncantationPuissante);
    }
  }
}
