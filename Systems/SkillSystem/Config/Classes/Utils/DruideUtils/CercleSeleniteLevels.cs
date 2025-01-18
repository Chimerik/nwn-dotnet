using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCercleSeleniteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(6).SetPlayerOverride(player.oid, "Cercle Sélénite");
          player.oid.SetTextureOverride("druide", "druide_lune");

          player.LearnClassSkill(CustomSkill.DruideFormeDeLune);
          player.LearnClassSkill(CustomSkill.FormeSauvageOurs);

          player.LearnAlwaysPreparedSpell((int)Spell.CureModerateWounds, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.RayonDeLune, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.LueurEtoilee, CustomClass.Druid);
          
          break;

        case 4: player.LearnClassSkill(CustomSkill.FormeSauvageCorbeau); break;
        case 5: player.LearnAlwaysPreparedSpell((int)Spell.SummonCreatureIii, CustomClass.Druid); break;

        case 6:
          player.LearnClassSkill(CustomSkill.DruideResilienceSauvage);
          player.LearnClassSkill(CustomSkill.DruideLuneRadieuse);
          break;

        case 7: player.LearnAlwaysPreparedSpell(CustomSpell.PuitsDeLune, CustomClass.Druid); break;
        case 8: player.LearnClassSkill(CustomSkill.FormeSauvageTigre); break;
        case 9: player.LearnAlwaysPreparedSpell((int)Spell.MassHeal, CustomClass.Druid); break;

        case 10:
          player.LearnClassSkill(CustomSkill.DruideProtectionNaturelle);
          player.LearnClassSkill(CustomSkill.FormeSauvageAir);
          player.LearnClassSkill(CustomSkill.FormeSauvageTerre);
          player.LearnClassSkill(CustomSkill.FormeSauvageFeu);
          player.LearnClassSkill(CustomSkill.FormeSauvageEau);
          break;

        case 14: player.LearnAlwaysPreparedSpell(CustomSpell.ChangementDapparence, CustomClass.Druid); break;
      }
    }
  }
}
