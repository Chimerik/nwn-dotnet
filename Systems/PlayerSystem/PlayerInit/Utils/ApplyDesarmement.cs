using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDesarmement()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCommotion))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipExpertiseDesarmement;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipDesarmement;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipExpertiseDesarmement;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipDesarmement;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Whip))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseDesarmement))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDesarmement, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDesarmement, 0);
        }
      }
    }
  }
}
