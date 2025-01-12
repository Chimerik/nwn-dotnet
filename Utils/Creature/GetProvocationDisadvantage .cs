using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetProvocationDisadvantage(uint effCreator, CNWSCreature target)
    {
      if (target.m_idSelf != effCreator)
      {
        LogUtils.LogMessage("Désavantage - Affecté par Provocation", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
