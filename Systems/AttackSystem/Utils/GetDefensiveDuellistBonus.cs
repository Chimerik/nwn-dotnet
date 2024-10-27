using Anvil.API;
using NWN.Native.API;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDefensiveDuellistBonus(CNWSCreature target, int rangedAttack, bool pactWeapon)
    {
      if(rangedAttack.ToBool() || !target.m_pStats.HasFeat((ushort)Feat.PrestigeDefensiveAwareness1).ToBool()
        || !target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo).ToBool())
        return 0;
      
      CNWSItem weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand);

      if (weapon is null || !weapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
        weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand);
      if (weapon is null || !weapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
        return 0;

      target.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);

      return GetCreatureWeaponProficiencyBonus(target, weapon, pactWeapon);
    }
  }
}
