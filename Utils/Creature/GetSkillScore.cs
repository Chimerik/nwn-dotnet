using System;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
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
          if (creature.KnowsFeat((Feat)CustomSkill.ToucheATout))
            return (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(creature) / 2), MidpointRounding.ToZero);

          switch (ability)
          {
            case Ability.Strength:
            case Ability.Dexterity:
            case Ability.Constitution:

              if (player.learnableSkills.TryGetValue(CustomSkill.FighterChampion, out LearnableSkill champion) && champion.currentLevel > 6)
                return (int)Math.Round((double)(NativeUtils.GetCreatureProficiencyBonus(creature) / 2), MidpointRounding.AwayFromZero);

              break;
          }
        }
      }
      else
        score += NativeUtils.GetCreatureProficiencyBonus(creature);

      score += creature.GetAbilityModifier(ability);
      score += GetSkillEffectBonus(creature, skill);

      if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.WildMagicBienfaitEffectTag))
        score += NwRandom.Roll(Utils.random, 4);

      return score;
    }
  }
}
