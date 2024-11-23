using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleVieLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Vie");
          player.oid.SetTextureOverride("clerc", "domaine_vie");

          player.learnableSkills.TryAdd(CustomSkill.ClercDiscipleDeLaVie, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercDiscipleDeLaVie], player));
          player.learnableSkills[CustomSkill.ClercDiscipleDeLaVie].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercDiscipleDeLaVie].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.ClercPreservationDeLaVie, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercPreservationDeLaVie], player));
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].source.Add(Category.Class);

          player.LearnAlwaysPreparedSpell((int)Spell.Bless, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.CureModerateWounds, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.LesserRestoration, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Aid, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.LueurDespoir, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.RaiseDead, CustomClass.Clerc);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerriseurBeni, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerriseurBeni], player));
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].source.Add(Category.Class);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.GardienDeLaFoi, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.DeathWard, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.GreaterRestoration, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.MassHeal, CustomClass.Clerc);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerisonSupreme, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerisonSupreme], player));
          player.learnableSkills[CustomSkill.ClercGuerisonSupreme].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerisonSupreme].source.Add(Category.Class);

          break;
      }
    }
  }
}
