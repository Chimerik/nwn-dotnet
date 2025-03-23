using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeBrutale(NwCreature caster, int featId)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.RecklessAttackEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappeBrutale(featId), NwTimeSpan.FromRounds(1));
        caster.OnCreatureDamage -= BarbarianUtils.OnDamageFrappeBrutale;
        caster.OnCreatureDamage += BarbarianUtils.OnDamageFrappeBrutale;

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {StringUtils.ToWhitecolor(SkillSystem.learnableDictionary[featId].name)}", StringUtils.gold, true, true);
      }
      else
        caster.LoginPlayer?.SendServerMessage($"Vous devez être sous l'effet d'Attaque Téméraire pour utiliser cette capacité", ColorConstants.Red);

      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeBrutale, 0);
      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeSiderante, 0);
      caster.SetFeatRemainingUses((Feat)CustomSkill.FrappeDechirante, 0);
    }
  }
}
