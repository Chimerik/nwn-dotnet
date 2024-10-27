using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class OccultisteUtils
  {
    public static async void OnUnequipPacteDeLaLame(OnItemUnequip onUnequip)
    {
      NwCreature creature = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;
      
      if (creature is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem.ItemType))
        return;

      await NwTask.NextFrame();
      CreatureUtils.InitializeNumAttackPerRound(creature);
    }
    public static async void OnEquipPacteDeLaLame(OnItemEquip onEquip)
    {
      NwCreature creature = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (creature is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem.ItemType))
        return;

      await NwTask.NextFrame();
      CreatureUtils.InitializeNumAttackPerRound(creature);
    }
  }
}
