using System;
using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowProficiencyBonus(CNWSCreature target, Ability ability)
    {
      int proficiencyBonus = 0;

      if (PlayerSystem.Players.TryGetValue(target.m_idSelf, out PlayerSystem.Player player))
      {
        if (player.learnableSkills.TryGetValue(SkillSystem.GetSavingThrowIdFromAbility(ability), out LearnableSkill proficiency)
        && proficiency.currentLevel > 0)
          proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(target);
        else
        {
          if(ability == Ability.Constitution && target.m_pStats.HasFeat(CustomSkill.TemporaryConstitutionSaveProficiency).ToBool())
            proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(target);
          else if(target.m_pStats.HasFeat(CustomSkill.ToucheATout).ToBool())
            proficiencyBonus = (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(target) / 2), MidpointRounding.ToZero);
        }
      }
      else
        proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(target);

      return proficiencyBonus;
    }
  }
}
