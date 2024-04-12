using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleInvocationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Invocateur");
          player.oid.SetTextureOverride("wizard", "invocation");

          player.learnableSkills.TryAdd(CustomSkill.EvocateurFaconneurDeSorts, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EvocateurFaconneurDeSorts], player));
          player.learnableSkills[CustomSkill.EvocateurFaconneurDeSorts].LevelUp(player);
          player.learnableSkills[CustomSkill.EvocateurFaconneurDeSorts].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.InvocationPermutation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvocationPermutation], player));
          player.learnableSkills[CustomSkill.InvocationPermutation].LevelUp(player);
          player.learnableSkills[CustomSkill.InvocationPermutation].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.InvocationConcentration, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvocationConcentration], player));
          player.learnableSkills[CustomSkill.InvocationConcentration].LevelUp(player);
          player.learnableSkills[CustomSkill.InvocationConcentration].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.InvocationSupreme, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvocationSupreme], player));
          player.learnableSkills[CustomSkill.InvocationSupreme].LevelUp(player);
          player.learnableSkills[CustomSkill.InvocationSupreme].source.Add(Category.Class);

          break;
      }
    }
  }
}
