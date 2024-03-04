using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBoneChillDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      if ((RacialType)creature.m_pStats.m_nRace == RacialType.Undead && eff.m_sCustomTag.CompareNoCase(EffectSystem.boneChillEffectExoTag) > 0)
      {
        LogUtils.LogMessage("Désavantage - Mort-vivant sous l'effet de Frisson Glacial", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
