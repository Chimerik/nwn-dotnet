using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsHeavyWeapon(BaseItemType itemType)
    {
      return itemType switch
      {
        BaseItemType.TwoBladedSword or BaseItemType.DireMace or BaseItemType.Doubleaxe or BaseItemType.Halberd 
        or BaseItemType.Greataxe or BaseItemType.Greatsword or BaseItemType.HeavyCrossbow or BaseItemType.HeavyFlail 
        or BaseItemType.Longbow => true,
        _ => false,
      };
    }
  }
}
