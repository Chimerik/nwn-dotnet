using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Monk
  {
    public static void HandlePaumeLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Voie de la Paume");

          player.learnableSkills.TryAdd(CustomSkill.MonkPaumeTechnique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkPaumeTechnique], player));
          player.learnableSkills[CustomSkill.MonkPaumeTechnique].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkPaumeTechnique].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.MonkPlenitude, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkPlenitude], player));
          player.learnableSkills[CustomSkill.MonkPlenitude].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkPlenitude].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkManifestationAme, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkManifestationAme], player));
          player.learnableSkills[CustomSkill.MonkManifestationAme].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkManifestationAme].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkManifestationCorps, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkManifestationCorps], player));
          player.learnableSkills[CustomSkill.MonkManifestationCorps].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkManifestationCorps].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkManifestationEsprit, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkManifestationEsprit], player));
          player.learnableSkills[CustomSkill.MonkManifestationEsprit].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkManifestationEsprit].source.Add(Category.Class);

          break;

        case 9:

          player.learnableSkills.TryAdd(CustomSkill.MonkResonanceKi, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkResonanceKi], player));
          player.learnableSkills[CustomSkill.MonkResonanceKi].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkResonanceKi].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.MonkExplosionKi, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkExplosionKi], player));
          player.learnableSkills[CustomSkill.MonkExplosionKi].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkExplosionKi].source.Add(Category.Class);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.MonkPaumeVibratoire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MonkPaumeVibratoire], player));
          player.learnableSkills[CustomSkill.MonkPaumeVibratoire].LevelUp(player);
          player.learnableSkills[CustomSkill.MonkPaumeVibratoire].source.Add(Category.Class);

          break;
      }
    }
  }
}
