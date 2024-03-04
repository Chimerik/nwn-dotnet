using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetArmorShieldDisadvantage(Native.API.CGameEffect eff, Ability attackStat)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.shieldArmorDisadvantageEffectExoTag).ToBool()
        && (attackStat == Ability.Strength || attackStat == Ability.Dexterity))
      {
        LogUtils.LogMessage($"Désavantage - Attaque (force ou dex) en portant un bouclier ou armure non maîtrisée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
