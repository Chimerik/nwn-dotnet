using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetDuelForceDisadvantage(uint effCreator, CNWSCreature target)
    {
      if (effCreator != target.m_idSelf)
      {
        LogUtils.LogMessage("Désavantage - Duel Forcé", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
