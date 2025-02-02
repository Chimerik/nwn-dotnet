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
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipBriseEchine;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipBriseEchine;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipBriseEchine;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipBriseEchine;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.DireMace, BaseItemType.Warhammer))
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
