using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFaveurDuMalinBonusDamage(CNWSCreature creature)
    {
      int bonusDamage = 0;
      var eff = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.faveurDuMalinEffectExoTag).ToBool() && e.GetInteger(5) == CustomSpell.FaveurDuMalinDegats);
      
      if(eff is not null)
      {
        bonusDamage = NwRandom.Roll(Utils.random, 10);
        creature.RemoveEffect(eff);
        LogUtils.LogMessage($"Faveur du malin dégâts : +{bonusDamage}", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
