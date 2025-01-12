using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetIlluminationProtectriceDisadvantage(CGameEffect eff, CNWSCreature target)
    {
      LogUtils.LogMessage("Désavantage - Illumination Protectrice", LogUtils.LogType.Combat);
      EffectUtils.DelayEffectRemoval(target, eff);
      return true;   
    }
  }
}
