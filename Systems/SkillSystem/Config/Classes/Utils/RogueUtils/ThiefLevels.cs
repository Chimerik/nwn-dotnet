using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleThiefLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Voleur");
          player.oid.SetTextureOverride("rogue", "thief");

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MainLeste)))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.MainLeste));

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.ThiefDiscretionSupreme, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ThiefDiscretionSupreme], player));
          player.learnableSkills[CustomSkill.ThiefDiscretionSupreme].LevelUp(player);
          player.learnableSkills[CustomSkill.ThiefDiscretionSupreme].source.Add(Category.Class);

          break;

        case 13:

          

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ThiefReflex, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ThiefReflex], player));
          player.learnableSkills[CustomSkill.ThiefReflex].LevelUp(player);
          player.learnableSkills[CustomSkill.ThiefReflex].source.Add(Category.Class);

          break;
      }
    }
  }
}
