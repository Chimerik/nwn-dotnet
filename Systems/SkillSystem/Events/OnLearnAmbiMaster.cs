using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAmbiMaster(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.Ambidexterity));

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyAmbiMaster;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveAmbiMaster;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyAmbiMaster;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveAmbiMaster;

      if (ItemUtils.IsWeapon(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem) && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);

      return true;
    }
  }
}
