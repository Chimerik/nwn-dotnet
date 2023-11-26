using Anvil.API;
using NWN.Native.API;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    private static readonly CExoString isFinesseWeaponVariable = "_IS_FINESSE_WEAPON".ToExoString();
    public static int GetDefensiveDuellistBonus(CNWSCreature target, int rangedAttack)
    {
      if(rangedAttack.ToBool() || !target.m_pStats.HasFeat((ushort)Feat.PrestigeDefensiveAwareness1).ToBool()
        || !target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo).ToBool())
        return 0;
      
      CNWSItem weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand);

      if (weapon is null || !weapon.m_ScriptVars.GetInt(isFinesseWeaponVariable).ToBool())
        weapon = target?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand);
      if (weapon is null || !weapon.m_ScriptVars.GetInt(isFinesseWeaponVariable).ToBool())
        return 0;

      target.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);

      return GetCreatureWeaponProficiencyBonus(target, weapon);
    }
  }
}
