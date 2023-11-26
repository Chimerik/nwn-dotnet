using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMaitreArmureIntermediaire()
      {
        if (learnableSkills.TryGetValue(CustomSkill.MaitreArmureIntermediaire, out LearnableSkill master) && master.currentLevel > 0)
        {
          NwItem armor = oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

          if (armor is not null && armor.BaseACValue > 2 && armor.BaseACValue < 6)
          {
            oid.LoginCreature.OnHeartbeat -= ItemSystem.OnHBCheckMaitreArmureIntermediaire;
            oid.LoginCreature.OnHeartbeat += ItemSystem.OnHBCheckMaitreArmureIntermediaire;

            oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMaitreArmureIntermediaire;
            oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMaitreArmureIntermediaire;

            if (oid.LoginCreature.GetAbilityModifier(Ability.Dexterity) > 2)
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.maitreArmureIntermediaire);
          }
          else
          {
            oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMaitreArmureIntermediaire;
            oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMaitreArmureIntermediaire;
          }
        }
      }
    }
  }
}
