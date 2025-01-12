using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFaveurDuMalinBonus(CNWSCreature attacker, CGameEffect eff, List<string> noStack)
    {
      int boonBonus = Utils.Roll(10);
      EffectUtils.DelayEffectRemoval(attacker, eff);
      LogUtils.LogMessage($"Faveur du Malin Attaque : +{boonBonus} BA", LogUtils.LogType.Combat);

      noStack.Add(EffectSystem.FaveurDuMalinEffectTag);

      return boonBonus;
    }
  }
}
