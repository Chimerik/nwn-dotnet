using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSorcellerieInneeAdvantage(NwSpell spell)
    {
      if (spell is not null && spell.GetSpellLevelForClass(ClassType.Sorcerer) < 15)
      {
        LogUtils.LogMessage("Avantage - Sorcellerie Innée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
