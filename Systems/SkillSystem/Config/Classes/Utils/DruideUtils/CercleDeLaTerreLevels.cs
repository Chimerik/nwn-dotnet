using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCercleTerreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(3).SetPlayerOverride(player.oid, "Cercle de la Terre");
          player.oid.SetTextureOverride("druide", "druide_terre");
          
          /*player.learnableSkills.TryAdd(CustomSkill.EnsoMagieTempetueuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoMagieTempetueuse], player));
          player.learnableSkills[CustomSkill.EnsoMagieTempetueuse].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoMagieTempetueuse].source.Add(Category.Class);*/

          break;

        case 6:

          //EnsoUtils.LearnSorcerySpell(player, (int)Spell.Balagarnsironhorn);
          //EnsoUtils.LearnSorcerySpell(player, CustomSpell.TempeteDeNeige);

          break;

        case 11:


          break;

        case 18:


          break;
      }
    }
  }
}
