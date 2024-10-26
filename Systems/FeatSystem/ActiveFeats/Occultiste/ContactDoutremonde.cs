using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ContactDoutremonde(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }
      
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.OccultisteContactDoutremonde);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Contact d'Outremonde", StringUtils.gold, true, true);
    }
  }
}
