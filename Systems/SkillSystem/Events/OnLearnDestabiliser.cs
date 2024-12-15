using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDestabiliser(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseDestabiliser))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseDestabiliser);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipDestabiliser;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipDestabiliser;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipDestabiliser;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipDestabiliser;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Quarterstaff, BaseItemType.MagicStaff, BaseItemType.Whip))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseDestabiliser))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 100);
      }

      return true;
    }
  }
}
