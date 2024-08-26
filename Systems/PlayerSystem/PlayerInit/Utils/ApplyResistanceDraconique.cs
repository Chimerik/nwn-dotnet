using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyResistanceDraconique()
      {
        if (learnableSkills.ContainsKey(CustomSkill.EnsorceleurLigneeDraconique))
        {
          oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipResistanceDraconique;
          oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipResistanceDraconique;
          oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipResistanceDraconique;
          oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipResistanceDraconique;

          NwItem armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

          if (armor is null || armor.BaseACValue < 1)
          {
            oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
            oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckResistanceDraconique;

            if (oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 0 && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ResistanceDraconiqueEffectTag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetResistanceDraconiqueEffect(oid.LoginCreature.GetAbilityModifier(Ability.Charisma)));
          }
        }
      }
    }
  }
}
