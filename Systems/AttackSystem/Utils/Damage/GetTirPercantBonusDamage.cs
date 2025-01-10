namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetTirPercantBonusDamage()
    {
      LogUtils.LogMessage("Tir Perçant : Dégâts +2", LogUtils.LogType.Combat);
      return 2;
    }
  }
}
