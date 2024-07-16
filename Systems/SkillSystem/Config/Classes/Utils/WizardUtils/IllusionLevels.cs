using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleIllusionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Illusionniste");
          player.oid.SetTextureOverride("wizard", "illusion");

          if (player.learnableSpells.TryGetValue(CustomSpell.IllusionMineure, out var learnable))
          {
            if (learnable.learntFromClasses.Any(c => c == (int)ClassType.Wizard))
            {
              if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, 0));
              else ((SpellSelectionWindow)cantrip1).CreateWindow(ClassType.Wizard, 1, 0);
            }
            else
            { 
              learnable.learntFromClasses.Add((int)ClassType.Wizard);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.IllusionMineure], (int)ClassType.Wizard);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);

            player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Illusion Mineure")}", ColorConstants.Orange);
          }

          player.learnableSkills.TryAdd(CustomSkill.WizardIllusionAmelioree, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WizardIllusionAmelioree], player));
          player.learnableSkills[CustomSkill.WizardIllusionAmelioree].LevelUp(player);
          player.learnableSkills[CustomSkill.WizardIllusionAmelioree].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.WizardIllusionMalleable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WizardIllusionMalleable], player));
          player.learnableSkills[CustomSkill.WizardIllusionMalleable].LevelUp(player);
          player.learnableSkills[CustomSkill.WizardIllusionMalleable].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.IllusionVoirLinvisible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IllusionVoirLinvisible], player));
          player.learnableSkills[CustomSkill.IllusionVoirLinvisible].LevelUp(player);
          player.learnableSkills[CustomSkill.IllusionVoirLinvisible].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.IllusionDouble, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.IllusionDouble], player));
          player.learnableSkills[CustomSkill.IllusionDouble].LevelUp(player);
          player.learnableSkills[CustomSkill.IllusionDouble].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.WizardRealiteIllusoire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WizardRealiteIllusoire], player));
          player.learnableSkills[CustomSkill.WizardRealiteIllusoire].LevelUp(player);
          player.learnableSkills[CustomSkill.WizardRealiteIllusoire].source.Add(Category.Class);

          break;
      }
    }
  }
}
