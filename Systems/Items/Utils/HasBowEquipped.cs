using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool HasBowEquipped(BaseItemType? itemType)
    {
      return itemType switch
      {
        BaseItemType.Shortbow or BaseItemType.Longbow => true,
        _ => false,
      };
    }
  }
}
