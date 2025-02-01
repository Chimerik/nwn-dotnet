using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCharge()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCommotion))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipCharge;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipCharge;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCharge;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipCharge;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Halberd, BaseItemType.ShortSpear)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCharge))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 0);
        }
      }
    }
  }
}
