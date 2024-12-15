using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFendre()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseFendre))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipFendre;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipFendre;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipFendre;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipFendre;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Doubleaxe, BaseItemType.Greatsword, BaseItemType.TwoBladedSword, BaseItemType.Greataxe, BaseItemType.DwarvenWaraxe, BaseItemType.Halberd, BaseItemType.Scythe)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFendre))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 0);
        }
      }
    }
  }
}
