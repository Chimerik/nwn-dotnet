using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleChasseurLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Chasseurs");
          player.oid.SetTextureOverride("ranger", "chasseur");

          if (!player.windows.TryGetValue("hunterProieSelection", out var value)) player.windows.Add("hunterProieSelection", new HunterProieSelectionWindow(player));
          else ((HunterProieSelectionWindow)value).CreateWindow();

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.RangerChasseurBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.RangerChasseurBonusAttack], player));
          player.learnableSkills[CustomSkill.RangerChasseurBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.RangerChasseurBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1;

          break;

        case 7:

          if (!player.windows.TryGetValue("hunterTactiqueDefensiveSelection", out var def)) player.windows.Add("hunterTactiqueDefensiveSelection", new HunterTactiqueDefensiveSelectionWindow(player));
          else ((HunterTactiqueDefensiveSelectionWindow)def).CreateWindow();

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.ChasseurVolee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ChasseurVolee], player));
          player.learnableSkills[CustomSkill.ChasseurVolee].LevelUp(player);
          player.learnableSkills[CustomSkill.ChasseurVolee].source.Add(Category.Class);

          break;

        case 15:

          if (!player.windows.TryGetValue("hunterDefenseSuperieureSelection", out var defsup)) player.windows.Add("hunterDefenseSuperieureSelection", new HunterDefenseSuperieureSelectionWindow(player));
          else ((HunterDefenseSuperieureSelectionWindow)defsup).CreateWindow();

          break;
      }
    }
  }
}
