using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFrappeDuPommeau()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseFrappeDuPommeau))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipFrappeDuPommeau;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipFrappeDuPommeau;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipFrappeDuPommeau;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipFrappeDuPommeau;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Longsword, BaseItemType.Bastardsword, BaseItemType.Greatsword))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFrappeDuPommeau))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 0);
        }
      }
    }
  }
}
