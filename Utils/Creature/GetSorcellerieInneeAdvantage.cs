using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSorcellerieInneeAdvantage(Native.API.CGameEffect eff, NwSpell spell)
    {
      if (spell is not null && spell.GetSpellLevelForClass(ClassType.Sorcerer) < 15 &&
        eff.m_sCustomTag.CompareNoCase(EffectSystem.SorcellerieInneeEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Sorcellerie Innée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
