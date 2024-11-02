using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleGrandAncienLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Grand Ancien");
          player.oid.SetTextureOverride("occultiste", "warlock_ancien");

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.BurningHands, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.CureModerateWounds, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Light, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LesserRestoration, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Flare, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.EclairTracant, CustomClass.Occultiste);
          

          player.learnableSkills.TryAdd(CustomSkill.LueurDeGuérison, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LueurDeGuérison], player));
          player.learnableSkills[CustomSkill.LueurDeGuérison].LevelUp(player);
          player.learnableSkills[CustomSkill.LueurDeGuérison].source.Add(Category.Class);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LumiereDuJour, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.RaiseDead, CustomClass.Occultiste);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.AmeRadieuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AmeRadieuse], player));
          player.learnableSkills[CustomSkill.AmeRadieuse].LevelUp(player);
          player.learnableSkills[CustomSkill.AmeRadieuse].source.Add(Category.Class);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.WallOfFire, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.GardienDeLaFoi, CustomClass.Occultiste);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.GreaterRestoration, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LesserPlanarBinding, CustomClass.Occultiste);

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
