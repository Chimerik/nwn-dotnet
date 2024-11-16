using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetIlluminationProtectriceDisadvantage(CGameEffect eff)
    {
      if (eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.IlluminationProtectriceEffectExoTag, EffectSystem.IlluminationProtectriceEffectExoTag.GetLength()) > 0)
      {
        LogUtils.LogMessage("Désavantage - Illumination Protectrice", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;     
    }
  }
}
