using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBriseEchine()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseBriseEchine))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipBriseEchine;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipBriseEchine;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipBriseEchine;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipBriseEchine;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.DireMace, BaseItemType.Warhammer))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseBriseEchine))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseBriseEchine, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseBriseEchine, 0);
        }
      }
    }
  }
}
