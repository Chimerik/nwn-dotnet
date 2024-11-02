using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ResilienceFielleuse(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      // ouvrir fenêtre choix résistance dégâts
      
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ResilienceFielleuse);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Résilience Fielleuse", StringUtils.brightPurple, true, true);
    }
  }
}
