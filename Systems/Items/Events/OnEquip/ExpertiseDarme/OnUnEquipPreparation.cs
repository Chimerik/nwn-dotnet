
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipPreparation(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Katana, BaseItemType.Scythe))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertisePreparation))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 0);
    }
  }
}
