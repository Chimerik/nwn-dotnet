using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetArmorShieldDisadvantage(Native.API.CGameEffect eff, Ability attackStat)
    {
      return eff.m_sCustomTag.CompareNoCase(StringUtils.shieldArmorDisadvantageEffectExoTag) > 0 
        && (attackStat == Ability.Strength || attackStat == Ability.Dexterity);
    }
  }
}
