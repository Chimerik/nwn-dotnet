using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleCelesteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Céleste");
          player.oid.SetTextureOverride("occultiste", "warlock_celeste");

          player.LearnAlwaysPreparedSpell((int)Spell.Aid, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.CureModerateWounds, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.Light, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.LesserRestoration, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.Flare, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.EclairTracant, CustomClass.Occultiste);
          

          player.learnableSkills.TryAdd(CustomSkill.LueurDeGuérison, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LueurDeGuérison], player));
          player.learnableSkills[CustomSkill.LueurDeGuérison].LevelUp(player);
          player.learnableSkills[CustomSkill.LueurDeGuérison].source.Add(Category.Class);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.LumiereDuJour, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.RaiseDead, CustomClass.Occultiste);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.AmeRadieuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AmeRadieuse], player));
          player.learnableSkills[CustomSkill.AmeRadieuse].LevelUp(player);
          player.learnableSkills[CustomSkill.AmeRadieuse].source.Add(Category.Class);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.WallOfFire, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.GardienDeLaFoi, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.GreaterRestoration, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.LesserPlanarBinding, CustomClass.Occultiste);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.ResilienceCeleste, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ResilienceCeleste], player));
          player.learnableSkills[CustomSkill.ResilienceCeleste].LevelUp(player);
          player.learnableSkills[CustomSkill.ResilienceCeleste].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.VengeanceCalcinante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.VengeanceCalcinante], player));
          player.learnableSkills[CustomSkill.VengeanceCalcinante].LevelUp(player);
          player.learnableSkills[CustomSkill.VengeanceCalcinante].source.Add(Category.Class);

          break;
      }
    }
  }
}
