using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyEntaille()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseEntaille))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipEntaille;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipEntaille;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipEntaille;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipEntaille;

          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(secondWeapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.LightHammer, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseEntaille))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 0);
        }
      }
    }
  }
}
