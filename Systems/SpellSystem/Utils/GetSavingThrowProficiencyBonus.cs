using System;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowProficiencyBonus(CNWSCreature target, Ability ability)
    {
      if (PlayerSystem.Players.TryGetValue(target.m_idSelf, out PlayerSystem.Player player))
      {
        if (player.learnableSkills.TryGetValue(SkillSystem.GetSavingThrowIdFromAbility(ability), out LearnableSkill proficiency)
        && proficiency.currentLevel > 0)
          return NativeUtils.GetCreatureProficiencyBonus(target);
        else
        {
          switch(ability) 
          {
            case Ability.Strength:
            case Ability.Dexterity:
            case Ability.Constitution:
              
              if (player.learnableSkills.TryGetValue(CustomClass.Champion, out LearnableSkill champion) && champion.currentLevel > 6)
                return (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(target) / 2), MidpointRounding.AwayFromZero);
              
              break;
          }
        }
      }
      else
        return NativeUtils.GetCreatureProficiencyBonus(target);

      return 0;
    }
  }
}
