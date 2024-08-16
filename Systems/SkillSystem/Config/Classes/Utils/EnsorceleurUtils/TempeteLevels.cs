using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static void HandleTempeteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(9).SetPlayerOverride(player.oid, "Sorcellerie de la Tempête");
          player.oid.SetTextureOverride("ensorceleur", "enso_draconique");

          player.learnableSkills.TryAdd(CustomSkill.EnsoMagieTempetueuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoMagieTempetueuse], player));
          player.learnableSkills[CustomSkill.EnsoMagieTempetueuse].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoMagieTempetueuse].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.EnsoCoeurDeLaTempete, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoCoeurDeLaTempete], player));
          player.learnableSkills[CustomSkill.EnsoCoeurDeLaTempete].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoCoeurDeLaTempete].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.EnsoGuideTempete, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoGuideTempete], player));
          player.learnableSkills[CustomSkill.EnsoGuideTempete].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoGuideTempete].source.Add(Category.Class);

          EnsoUtils.LearnSorcerySpell(player, (int)Spell.Balagarnsironhorn);
          EnsoUtils.LearnSorcerySpell(player, (int)Spell.GustOfWind);
          EnsoUtils.LearnSorcerySpell(player, (int)Spell.CallLightning);
          EnsoUtils.LearnSorcerySpell(player, CustomSpell.TempeteDeNeige);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.EnsoFureurTempete, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoFureurTempete], player));
          player.learnableSkills[CustomSkill.EnsoFureurTempete].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoFureurTempete].source.Add(Category.Class);

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.EnsoAmeDesVents, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoAmeDesVents], player));
          player.learnableSkills[CustomSkill.EnsoAmeDesVents].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoAmeDesVents].source.Add(Category.Class);

          break;
      }
    }
  }
}
