using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleEvocationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Evocateur");
          player.oid.SetTextureOverride("wizard", "evocation");

          player.learnableSkills.TryAdd(CustomSkill.EvocateurFaconneurDeSorts, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EvocateurFaconneurDeSorts], player));
          player.learnableSkills[CustomSkill.EvocateurFaconneurDeSorts].LevelUp(player);
          player.learnableSkills[CustomSkill.EvocateurFaconneurDeSorts].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.EvocateurToursPuissants, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EvocateurToursPuissants], player));
          player.learnableSkills[CustomSkill.EvocateurToursPuissants].LevelUp(player);
          player.learnableSkills[CustomSkill.EvocateurToursPuissants].source.Add(Category.Class);
          
          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.EvocateurSuperieur, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EvocateurSuperieur], player));
          player.learnableSkills[CustomSkill.EvocateurSuperieur].LevelUp(player);
          player.learnableSkills[CustomSkill.EvocateurSuperieur].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.EvocateurSurcharge, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EvocateurSurcharge], player));
          player.learnableSkills[CustomSkill.EvocateurSurcharge].LevelUp(player);
          player.learnableSkills[CustomSkill.EvocateurSurcharge].source.Add(Category.Class);

          break;
      }
    }
  }
}
