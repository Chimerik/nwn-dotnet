using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnResistanceDraconique(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipResistanceDraconique;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipResistanceDraconique;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipResistanceDraconique;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipResistanceDraconique;

      NwItem armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 1)
      {
        player.oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
        player.oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckResistanceDraconique;

        if (player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 0 && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ResistanceDraconiqueEffectTag))
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetResistanceDraconiqueEffect(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma)));
      }

      return true;
    }
  }
}
