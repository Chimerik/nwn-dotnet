using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetFaerieFireAdvantage(CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.faerieFireEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Cible affectée par Lueurs Féeriques", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
