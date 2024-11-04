using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static Anvil.API.Ability GetAttackAbility(CNWSCreature creature, CNWSCombatAttackData attackData, CNWSItem attackWeapon)
    {
      Anvil.API.Ability attackStat = Anvil.API.Ability.Strength;
      int dexBonus = GetAbilityModifier(creature, Anvil.API.Ability.Dexterity);
      int strBonus = GetAbilityModifier(creature, Anvil.API.Ability.Strength);

      if (attackWeapon is not null)
      {
        if (attackWeapon.m_ScriptVars.GetObject(CreatureUtils.PacteDeLaLameVariableExo) == creature.m_idSelf)
        {
          attackStat = Anvil.API.Ability.Charisma;
          LogUtils.LogMessage($"Occultiste - Arme du Pacte de la Lame", LogUtils.LogType.Combat);
        }
        else if ((dexBonus > strBonus && attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable) != 0)
          || attackData.m_bRangedAttack.ToBool())
        {
          attackStat = Anvil.API.Ability.Dexterity;
        }
        else if(creature.m_pStats.GetNumLevelsOfClass(CustomClass.Monk) > 0 && NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).IsMonkWeapon)
        {
          attackStat = dexBonus > strBonus ? Anvil.API.Ability.Dexterity : Anvil.API.Ability.Strength;
        }
      }
      else if(creature.m_pStats.GetNumLevelsOfClass(CustomClass.Monk) > 0)
      {
        attackStat = dexBonus > strBonus ? Anvil.API.Ability.Dexterity : Anvil.API.Ability.Strength;
      }   

      return attackStat;
    }
  }
}
