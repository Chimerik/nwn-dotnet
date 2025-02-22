using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Inflexible(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.InflexibleEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage("Vous bénéficiez déjà de cet effet", ColorConstants.Orange);
        return;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Inflexible);
      FeatUtils.DecrementFeatUses(caster, CustomSkill.FighterInflexible);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Inflexible".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
  }
}
