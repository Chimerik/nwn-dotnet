using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMoquerieVicieuseDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      LogUtils.LogMessage("Désavantage - Moquerie Vicieuse", LogUtils.LogType.Combat);
      EffectUtils.DelayEffectRemoval(creature, eff);
      return true;
    }
  }
}
