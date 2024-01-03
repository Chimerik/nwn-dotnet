using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsDualWieldingLightWeapon(CNWSItem attackWeapon, int creatureSize, NwItem offHandWeapon)
    {
      if (attackWeapon is null || creatureSize < 1)
        return false;

      NwBaseItem baseAttackWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);

      return baseAttackWeapon.ItemType switch
      {
        BaseItemType.DireMace or BaseItemType.Doubleaxe or BaseItemType.TwoBladedSword => true,
        _ => offHandWeapon is not null
          && offHandWeapon.BaseItem.NumDamageDice > 0
          && offHandWeapon.BaseItem.WeaponSize > BaseItemWeaponSize.Unknown
          && (int)offHandWeapon.BaseItem.WeaponSize < creatureSize
      };
    }
  }
}
