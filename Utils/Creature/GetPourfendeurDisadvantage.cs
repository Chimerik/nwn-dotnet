using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetPourfendeurDisadvantage(Native.API.CGameEffect eff)
    {
      return eff.m_sCustomTag.CompareNoCase(EffectSystem.PourfendeurDisadvantageEffectExoTag).ToBool();
    }
  }
}
