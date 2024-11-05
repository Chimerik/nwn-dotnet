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
      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeBrutale, 1);
      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeSiderante, 1);
      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeDechirante, 1);

      if (caster.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike) && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappeFrenetique(caster), NwTimeSpan.FromRounds(1));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {StringUtils.ToWhitecolor("Frappe Frénétique")}", ColorConstants.Orange, true, true);
      }
      else
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {StringUtils.ToWhitecolor("Frappe Téméraire")}", ColorConstants.Orange, true, true);
    }
  }
}
