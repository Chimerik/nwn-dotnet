using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyTirPercant()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseTirPercant))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipTirPercant;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipTirPercant;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipTirPercant;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipTirPercant;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightCrossbow, BaseItemType.HeavyCrossbow, BaseItemType.Shuriken))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTirPercant))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTirPercant, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTirPercant, 0);
        }
      }
    }
  }
}
