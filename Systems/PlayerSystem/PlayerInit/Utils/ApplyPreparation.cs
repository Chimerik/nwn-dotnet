using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPreparation()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertisePreparation))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipPreparation;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipPreparation;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipPreparation;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipPreparation;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Katana, BaseItemType.Scythe))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertisePreparation))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 0);
        }
      }
    }
  }
}
