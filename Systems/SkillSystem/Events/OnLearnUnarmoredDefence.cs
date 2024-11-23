using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnUnarmoredDefence(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipUnarmoredDefence;

      if (!ItemUtils.IsArmor(player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest)))
      {
        player.oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        player.oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

        if (player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution) > 0 && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.UnarmoredDefenceEffectTag))
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(player.oid.LoginCreature.GetAbilityModifier(Ability.Constitution)));
      }

      return true;
    }
  }
}
