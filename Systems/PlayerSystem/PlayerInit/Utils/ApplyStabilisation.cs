
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyStabilisation()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseStabilisation))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipStabilisation;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipStabilisation;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipStabilisation;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipStabilisation;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.HeavyCrossbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseStabilisation))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseStabilisation, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseStabilisation, 0);
        }
      }
    }
  }
}
