using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAffaiblissement()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseAffaiblissement))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipAffaiblissement;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipAffaiblissement;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipAffaiblissement;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipAffaiblissement;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.HeavyFlail, BaseItemType.Rapier, BaseItemType.Whip, BaseItemType.Sling))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Rapier, BaseItemType.Whip)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseAffaiblissement))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 0);
        }
      }
    }
  }
}
