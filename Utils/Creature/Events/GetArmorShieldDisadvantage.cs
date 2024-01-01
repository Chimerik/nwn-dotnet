using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetArmorShieldDisadvantage(Native.API.CGameEffect eff, Ability attackStat)
    {
      return eff.m_sCustomTag.CompareNoCase(EffectSystem.shieldArmorDisadvantageEffectExoTag).ToBool()
        && (attackStat == Ability.Strength || attackStat == Ability.Dexterity);
    }
  }
}
