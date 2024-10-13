using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static int GetSanctuaireNaturelBonusSave(NwCreature target, Ability saveType)
    {
      int shieldBonus = 0;

      if (saveType == Ability.Dexterity && target.ActiveEffects.Any(e => e.Tag == EffectSystem.SanctuaireNaturelEffectTag))
      {
        shieldBonus += 2;
        LogUtils.LogMessage($"Sanctuaire Naturel : JDS +2", LogUtils.LogType.Combat);
      }

      return shieldBonus;
    }
  }
}
