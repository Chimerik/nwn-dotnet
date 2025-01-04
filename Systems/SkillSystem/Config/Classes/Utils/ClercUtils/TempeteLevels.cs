using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleTempeteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Tempête");
          player.oid.SetTextureOverride("clerc", "domaine_tempete");


          player.LearnClassSkill(CustomSkill.ClercFureurOuragan);
          player.LearnClassSkill(CustomSkill.ClercFureurDestructrice);

          player.LearnAlwaysPreparedSpell(CustomSpell.NappeDeBrouillard, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Balagarnsironhorn, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Fracassement, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.GustOfWind, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.TempeteDeNeige, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.CallLightning, CustomClass.Clerc);

          break;

        case 6: player.LearnClassSkill(CustomSkill.ClercElectrocution); break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.ControleDeLeau, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.IceStorm, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.FleauDinsectes, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.VagueDestructrice, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercEnfantDeLaTempete); break;
      }
    }
  }
}
