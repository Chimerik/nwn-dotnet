using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RecklessAttack(NwCreature caster)
    {
      if (!caster.ActiveEffects.Any(e => e.Tag == EffectSystem.RecklessAttackEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.RecklessAttackEffect, NwTimeSpan.FromRounds(1));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Frappe Téméraire".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
      }
    }
  }
}
