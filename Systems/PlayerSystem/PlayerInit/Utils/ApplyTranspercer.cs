using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyTranspercer()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseTranspercer))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipTranspercer;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipTranspercer;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipTranspercer;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipTranspercer;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword))
          || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTranspercer))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 0);
        }
      }
    }
  }
}
