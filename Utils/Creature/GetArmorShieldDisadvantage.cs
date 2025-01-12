using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetArmorShieldDisadvantage(Ability attackStat)
    {
      switch(attackStat)
      {
        case Ability.Strength:
        case Ability.Dexterity: LogUtils.LogMessage($"Désavantage - Attaque (force ou dex) en portant un bouclier ou armure non maîtrisée", LogUtils.LogType.Combat); return true;
        default: return false;
      }
    }
  }
}
