using Anvil.API;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetFouleeDombreAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.FouleeDombreffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Foulée d'ombre", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
