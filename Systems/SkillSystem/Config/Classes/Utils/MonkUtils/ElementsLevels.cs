using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static void HandleElementsLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(10).SetPlayerOverride(player.oid, "Voie des Éléments");
          player.oid.SetTextureOverride("monk", "monk_elements");

          player.learnableSkills.TryAdd(CustomSkill.MonkHarmony, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkHarmony], player));
          player.learnableSkills[CustomSkill.MonkHarmony].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkHarmony].source.Add(Category.Class);



          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.Elementaliste, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elementaliste], player));
          player.learnableSkills[CustomSkill.Elementaliste].LevelUp(player);
          player.learnableSkills[CustomSkill.Elementaliste].source.Add(Category.Class);

          player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(5, new int[] { (int)DamageType.Cold });
          player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(6, new int[] { (int)DamageType.Fire });
          player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(7, new int[] { (int)DamageType.Electrical });
          player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(8, new int[] { (int)DamageType.Bludgeoning });
          player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(9, new int[] { (int)DamageType.Sonic });

          break;
      }
    }
  }
}
