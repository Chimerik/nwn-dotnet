using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetDuelForceDisadvantage(CGameEffect eff, CNWSCreature target)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.DuelForceEffectExoTag).ToBool() 
        && eff.m_oidCreator != target.m_idSelf)
      {
        LogUtils.LogMessage("Désavantage - Duel Forcé", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
