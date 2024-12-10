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
              if (!player.windows.TryGetValue("cantripSelection", out var cantrip1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Wizard, 1));
              else ((CantripSelectionWindow)cantrip1).CreateWindow(ClassType.Wizard, 1);
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

          player.LearnClassSkill(CustomSkill.WizardIllusionAmelioree);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.WizardIllusionMalleable);
          player.LearnClassSkill(CustomSkill.IllusionVoirLinvisible);

          break;

        case 10: player.LearnClassSkill(CustomSkill.IllusionDouble); break;

        case 14: player.LearnClassSkill(CustomSkill.WizardRealiteIllusoire); break;
      }
    }
  }
}
