using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipArbaleteDePoing(OnItemEquip onEquip)
    {
      NwCreature creature = onEquip.EquippedBy;
      NwItem item = onEquip.Item;

      if (creature is null || item is null || item.BaseItem.ItemType != BaseItemType.Shuriken)
        return;

      if (item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) < 11)
        item.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Top, 11);

      if (item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) < 11)
        item.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Middle, 11);

      if (item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) < 11)
        item.Appearance.SetWeaponModel(ItemAppearanceWeaponModel.Bottom, 11);

      item.VisualTransform.Scale = 0.5f;
    }
  }
}
