using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TacticalMastery(NwCreature caster, OnUseFeat onUseFeat)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if(onUseFeat.TargetObject is not NwCreature target || caster == target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (caster.DistanceSquared(target) > 80)
      {
        caster.LoginPlayer?.SendServerMessage("Cible hors de portée", ColorConstants.Red);
        return;
      }

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.maitreTactique, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise Maître Tactique sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
    }
  }
}
