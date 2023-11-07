using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetProtectionStyleDisadvantage(CGameEffect eff)
    {
      return eff.m_sCustomTag == EffectSystem.ProtectionStyleEffectExoTag;
    }
  }
}
