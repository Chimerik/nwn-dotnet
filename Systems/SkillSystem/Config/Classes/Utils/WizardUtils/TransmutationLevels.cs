using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public const string TransmutationStoneVariable = "_TRANSMUTATION_STONE_AVAILABLE";
    public static void HandleTransmutationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Transmutateur");
          player.oid.SetTextureOverride("wizard", "transmutation");

          player.learnableSkills.TryAdd(CustomSkill.TransmutationAlchimieMineure, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TransmutationAlchimieMineure], player));
          player.learnableSkills[CustomSkill.TransmutationAlchimieMineure].LevelUp(player);
          player.learnableSkills[CustomSkill.TransmutationAlchimieMineure].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.TransmutationStone, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TransmutationStone], player));
          player.learnableSkills[CustomSkill.TransmutationStone].LevelUp(player);
          player.learnableSkills[CustomSkill.TransmutationStone].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.TransmutationMetamorphose, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TransmutationMetamorphose], player));
          player.learnableSkills[CustomSkill.TransmutationMetamorphose].LevelUp(player);
          player.learnableSkills[CustomSkill.TransmutationMetamorphose].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue((int)Spell.PolymorphSelf, out var learnable))
          {
            if (!learnable.learntFromClasses.Any(c => c == (int)ClassType.Wizard))
            {
              learnable.learntFromClasses.Add((int)ClassType.Wizard);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.PolymorphSelf], (int)ClassType.Wizard);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);

            player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Métamorphose")}", ColorConstants.Orange);
          }

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.TransmutationMaitre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TransmutationMaitre], player));
          player.learnableSkills[CustomSkill.TransmutationMaitre].LevelUp(player);
          player.learnableSkills[CustomSkill.TransmutationMaitre].source.Add(Category.Class);

          break;
      }
    }
  }
}
