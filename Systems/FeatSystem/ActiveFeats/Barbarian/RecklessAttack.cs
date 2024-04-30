using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RecklessAttack(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.RecklessAttackEffect, NwTimeSpan.FromRounds(1));
      caster.SetFeatRemainingUses((Feat)CustomSkill.BarbarianRecklessAttack, 0);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {StringUtils.ToWhitecolor("Frappe Téméraire")}", ColorConstants.Orange, true);
    }
  }
}
