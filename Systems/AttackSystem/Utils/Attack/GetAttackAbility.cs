using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static Anvil.API.Ability GetAttackAbility(CNWSCreature creature, bool isRangedAttack, CNWSItem attackWeapon)
    {
      Anvil.API.Ability attackStat = Anvil.API.Ability.Strength;
      int dexBonus = GetAbilityModifier(creature, Anvil.API.Ability.Dexterity);
      int strBonus = GetAbilityModifier(creature, Anvil.API.Ability.Strength);

      if (attackWeapon is not null)
      {
        if(GetCreatureWeaponProficiencyBonus(creature, attackWeapon) > 0)
        {
          var coupAuBut = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.CoupAuButAttackEffectTag);
          if(coupAuBut is not null)
            return (Anvil.API.Ability)coupAuBut.m_nCasterLevel;
        }

        if (attackWeapon.m_ScriptVars.GetObject(CreatureUtils.PacteDeLaLameVariableExo) == creature.m_idSelf)
        {
          attackStat = Anvil.API.Ability.Charisma;
          LogUtils.LogMessage($"Occultiste - Arme du Pacte de la Lame", LogUtils.LogType.Combat);
        }
        else if ((dexBonus > strBonus && attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable) != 0) || isRangedAttack)
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
      else if(isRangedAttack && creature.m_pStats.HasFeat(CustomSkill.FightingStyleArcherDeForce).ToBool()
        && Utils.In((int)attackWeapon.m_nBaseItem, (int)BaseItem.Shortbow, (int)BaseItem.Longbow))
      {
        attackStat = Anvil.API.Ability.Strength;
      }

      return attackStat;
    }
  }
}
