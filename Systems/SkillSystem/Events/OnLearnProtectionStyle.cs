using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProtectionStyle(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipApplyProtectionStyle;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipRemoveProtectionStyle;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipApplyProtectionStyle;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipRemoveProtectionStyle;

      switch (player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
          if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionStyleEffectTag))
            NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.protectionStyleAura));
          break;
      }

      return true;
    }
  }
}
