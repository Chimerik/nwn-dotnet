using NWN.Native.API;
using RacialType = NWN.Native.API.RacialType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSensDivinAdvantage(CNWSCreature target)
    {
      if(target is not null && Utils.In((RacialType)target.m_pStats.m_nRace, RacialType.Undead, RacialType.Outsider))
      {
        LogUtils.LogMessage("Avantage - Sens Divin", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
