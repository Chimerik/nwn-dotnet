using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreArmureIntermediaire(PlayerSystem.Player player, int customSkillId)
    {
      NwItem armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);

      if(armor is not null && armor.BaseACValue > 2 && armor.BaseACValue < 6) 
      {
        player.oid.LoginCreature.OnHeartbeat -= ItemSystem.OnHBCheckMaitreArmureIntermediaire;
        player.oid.LoginCreature.OnHeartbeat += ItemSystem.OnHBCheckMaitreArmureIntermediaire;

        player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMaitreArmureIntermediaire;
        player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMaitreArmureIntermediaire;

        if (player.oid.LoginCreature.GetAbilityModifier(Ability.Dexterity) > 2)
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.maitreArmureIntermediaire);
      }
      else
      {
        player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMaitreArmureIntermediaire;
        player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMaitreArmureIntermediaire;
      }

      return true;
    }
  }
}
