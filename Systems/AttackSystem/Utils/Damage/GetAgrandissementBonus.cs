namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAgrandissementBonus(bool isCritical)
    {
      int roll = Utils.Roll(4, isCritical ? 2 : 1);
      LogUtils.LogMessage($"Agrandissement : +{roll}", LogUtils.LogType.Combat);

      return roll;
    }
  }
}
