using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetChanceuxDisadvantage(CNWSCreature attacker, CNWSCreature target, CGameEffect eff)
    {
      if(eff.m_oidCreator == target.m_idSelf)
      {
        LogUtils.LogMessage("Désavantage - Chanceux", LogUtils.LogType.Combat);
        attacker.RemoveEffect(eff);
        return true;
      }

      return false;
    }
  }
}
