using System;
using Anvil.API;
using NWN.Systems;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetSkillScore(NwCreature creature, Ability ability, int skill)
    {
      int score = 0;

      if(creature.IsLoginPlayerCharacter && PlayerSystem.Players.TryGetValue(creature, out PlayerSystem.Player player))
      {
        if (player.learnableSkills.TryGetValue(skill, out LearnableSkill learnable) && learnable.currentLevel > 0)
        {
          score += NativeUtils.GetCreatureProficiencyBonus(creature);

          if (player.learnableSkills.TryGetValue(skill + 1, out LearnableSkill expertise) && expertise.currentLevel > 0)
            score *= 2;
        }
        else
        {
          switch (ability)
          {
            case Ability.Strength:
            case Ability.Dexterity:
            case Ability.Constitution:

              if (creature.GetClassInfo(NwClass.FromClassId(CustomClass.Champion))?.Level > 6)
                return (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(creature) / 2), MidpointRounding.AwayFromZero);

              break;
          }
        }

        return score + creature.GetAbilityModifier(ability);
      }

      return score += NativeUtils.GetCreatureProficiencyBonus(creature);
    }
  }
}
