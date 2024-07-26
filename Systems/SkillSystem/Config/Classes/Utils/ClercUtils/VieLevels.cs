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
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Vie");
          player.oid.SetTextureOverride("clerc", "domaine_vie");

          player.learnableSkills.TryAdd(CustomSkill.HeavyArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyArmorProficiency], player));
          player.learnableSkills[CustomSkill.HeavyArmorProficiency].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.ClercDiscipleDeLaVie, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercDiscipleDeLaVie], player));
          player.learnableSkills[CustomSkill.ClercDiscipleDeLaVie].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercDiscipleDeLaVie].source.Add(Category.Class);

          ClercUtils.LearnDomaineSpell(player, (int)Spell.Bless);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.CureModerateWounds);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercPreservationDeLaVie, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercPreservationDeLaVie], player));
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].source.Add(Category.Class);

          break;

        case 3:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.LesserRestoration);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.ShelgarnsPersistentBlade);

          break;

        case 5:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.LueurDespoir);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.RaiseDead);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerriseurBeni, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerriseurBeni], player));
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].source.Add(Category.Class);

          break;

        case 7:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.GardienDeLaFoi);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.DeathWard);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercVieFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercVieFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercVieFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercVieFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.GreaterRestoration);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.MassHeal);

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
