using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetIlluminationProtectriceDisadvantage(CGameEffect eff, CNWSCreature target)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.IlluminationProtectriceEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Illumination Protectrice", LogUtils.LogType.Combat);
        target.RemoveEffect(eff);
        return true;
      }

      return false;     
    }
  }
}
