using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsRangedWeaponInOptimalRange(BaseItemType itemType, float hitDistance)
    {
      LogUtils.LogMessage($"targetDistance : {hitDistance}", LogUtils.LogType.Combat);

      return itemType switch
      {
        BaseItemType.ThrowingAxe or BaseItemType.Dart => hitDistance < 6,
        BaseItemType.Shuriken or BaseItemType.Sling => hitDistance < 9,
        BaseItemType.LightCrossbow or BaseItemType.Shortbow => hitDistance < 24,
        BaseItemType.HeavyCrossbow => hitDistance < 30,
        BaseItemType.Longbow => hitDistance < 45,
        _ => false,
      };
    }
  }
}
