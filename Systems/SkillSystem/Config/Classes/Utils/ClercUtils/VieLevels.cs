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

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Bless, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.CureModerateWounds, CustomClass.Clerc);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercPreservationDeLaVie, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercPreservationDeLaVie], player));
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercPreservationDeLaVie].source.Add(Category.Class);

          break;

        case 3:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LesserRestoration, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ShelgarnsPersistentBlade, CustomClass.Clerc);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LueurDespoir, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.RaiseDead, CustomClass.Clerc);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerriseurBeni, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerriseurBeni], player));
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerriseurBeni].source.Add(Category.Class);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.GardienDeLaFoi, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DeathWard, CustomClass.Clerc);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercVieFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercVieFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercVieFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercVieFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.GreaterRestoration, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.MassHeal, CustomClass.Clerc);

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
