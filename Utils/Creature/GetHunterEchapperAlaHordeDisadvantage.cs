using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetHunterEchapperAlaHordeDisadvantage(CNWSCombatAttackData attackData)
    {
      if (attackData is null || attackData.m_nAttackType != 65002)
        return false;

      LogUtils.LogMessage("Désavantage - Echapper à la horde", LogUtils.LogType.Combat);

      return true;
    }
  }
}
