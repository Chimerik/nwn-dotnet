using Anvil.API;
using static NWN.Systems.PlayerSystem;

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

          player.LearnClassSkill(CustomSkill.LueurDeGuérison);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.LumiereDuJour, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.RaiseDead, CustomClass.Occultiste);

          break;

        case 6: player.LearnClassSkill(CustomSkill.AmeRadieuse); break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.WallOfFire, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.GardienDeLaFoi, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.GreaterRestoration, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.LesserPlanarBinding, CustomClass.Occultiste);

          break;

        case 10: player.LearnClassSkill(CustomSkill.ResilienceCeleste); break;
        case 14: player.LearnClassSkill(CustomSkill.VengeanceCalcinante); break;
      }
    }
  }
}
