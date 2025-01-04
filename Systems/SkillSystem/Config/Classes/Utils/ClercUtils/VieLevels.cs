using Anvil.API;
using static NWN.Systems.PlayerSystem;

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

          player.LearnClassSkill(CustomSkill.ClercDiscipleDeLaVie);
          player.LearnClassSkill(CustomSkill.ClercPreservationDeLaVie);

          player.LearnAlwaysPreparedSpell((int)Spell.Bless, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.CureModerateWounds, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.LesserRestoration, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Aid, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.LueurDespoir, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.RaiseDead, CustomClass.Clerc);

          break;

        case 6: player.LearnClassSkill(CustomSkill.ClercDiscipleDeLaVie); break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.GardienDeLaFoi, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.DeathWard, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.GreaterRestoration, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.MassHeal, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercGuerisonSupreme); break;
      }
    }
  }
}
