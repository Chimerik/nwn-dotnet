using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRayonAffaiblissantDisadvantage(Ability attackStat)
    {
      switch(attackStat)
      {
        case Ability.Strength: LogUtils.LogMessage("Désavantage - Rayon Affaiblissant", LogUtils.LogType.Combat); return true;
        default: return false;
      }
    }
  }
}
