using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static int GetResilienceSauvageBonusSave(NwCreature target, Ability saveType)
    {
      int shieldBonus = 0;
      int wisMod = target.GetAbilityModifier(Ability.Wisdom);

      if (saveType == Ability.Constitution && wisMod > 0 && target.KnowsFeat((Feat)CustomSkill.DruideResilienceSauvage)  
        && target.ActiveEffects.Any(e => e.Tag == EffectSystem.PolymorphEffectTag))
      {
        shieldBonus += wisMod;
        LogUtils.LogMessage($"Résilience Sauvage : JDS +{wisMod}", LogUtils.LogType.Combat);
      }

      return shieldBonus;
    }
  }
}
