namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetTranspercerBonusDamage()
    {
      LogUtils.LogMessage("Transpercer : Dégâts +2", LogUtils.LogType.Combat);
      return 2;
    }
  }
}
