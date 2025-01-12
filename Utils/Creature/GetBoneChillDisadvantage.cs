using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBoneChillDisadvantage(CNWSCreature creature)
    {
      if ((RacialType)creature.m_pStats.m_nRace == RacialType.Undead)
      {
        LogUtils.LogMessage("Désavantage - Mort-vivant sous l'effet de Frisson Glacial", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
