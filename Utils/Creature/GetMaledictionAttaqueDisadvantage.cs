using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMaledictionAttaqueDisadvantage(uint effCreator, CNWSCreature target)
    {
      if (effCreator == target.m_idSelf)
      {
        LogUtils.LogMessage("Désavantage - Malédiction sur l'attaque", LogUtils.LogType.Combat);
        return true;
      }

      return false;
    }
  }
}
