using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetDefenseAdaptativeDisadvantage(uint effCreator, CNWSCreature target)
    {
      if (effCreator == target.m_idSelf)
        return false;

      LogUtils.LogMessage("Désavantage - Défense Adaptative", LogUtils.LogType.Combat);
      return true;
    }
  }
}
