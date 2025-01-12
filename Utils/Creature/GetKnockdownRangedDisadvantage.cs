namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetKnockdownRangedDisadvantage(bool rangedAttack)
    {
      if (!rangedAttack)
        return false;

      LogUtils.LogMessage("Désavantage - Attaque à distance sur une cible Déstabilisée", LogUtils.LogType.Combat);
      return true;    
    }
  }
}
