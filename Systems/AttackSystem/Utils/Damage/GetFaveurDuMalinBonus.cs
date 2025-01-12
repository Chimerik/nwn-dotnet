using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFaveurDuMalinBonus(CNWSCreature creature, CGameEffect eff, bool isCritical, List<string> noStack)
    {
      int roll = Utils.Roll(10, isCritical ? 2 : 1);
      creature.RemoveEffect(eff);
      LogUtils.LogMessage($"Faveur du malin dégâts : +{roll}", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.FaveurDuMalinEffectTag);

      return roll;
    }
  }
}
