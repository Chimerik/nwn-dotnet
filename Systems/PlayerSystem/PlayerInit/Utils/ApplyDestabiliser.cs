using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDestabiliser()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseBriseEchine))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipDestabiliser;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipDestabiliser;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipDestabiliser;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipDestabiliser;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Quarterstaff, BaseItemType.MagicStaff, BaseItemType.Whip))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseDestabiliser))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 0);
        }
      }
    }
  }
}
