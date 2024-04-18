using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPatienceDisadvantage(CGameEffect eff)
    {
      if (eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.MonkPatienceEffectExoTag, EffectSystem.MonkPatienceEffectExoTag.GetLength()) .ToBool())
      {
        LogUtils.LogMessage("Désavantage - Cible en mode patience", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;     
    }
  }
}
