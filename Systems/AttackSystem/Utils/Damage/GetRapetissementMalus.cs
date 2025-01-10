namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetRapetissementMalus()
    {
      int roll = Utils.Roll(4);
      LogUtils.LogMessage($"Rapetissement : +{roll}", LogUtils.LogType.Combat);

      return roll;
    }
  }
}
