using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DoubleVue(NwCreature caster, NwGameObject oTarget)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (oTarget is not NwCreature target || caster.IsReactionTypeHostile(target))
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      caster.LoginPlayer.AttachCamera(oTarget);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Double Vue", StringUtils.brightPurple, true, true);
    }
  }
}
