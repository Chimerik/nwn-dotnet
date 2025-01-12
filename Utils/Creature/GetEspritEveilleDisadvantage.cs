namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetEspritEveilleDisadvantage(uint effCreator, uint targetId)
    {
      if(effCreator == targetId)
      {
        LogUtils.LogMessage("Désavantage - Combattant Clairvoyant", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
