using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCercleTerreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(3).SetPlayerOverride(player.oid, "Cercle Tellurique");
          player.oid.SetTextureOverride("druide", "druide_terre");

          if (!player.windows.TryGetValue("terrainDeCercleSelection", out var value)) player.windows.Add("terrainDeCercleSelection", new TerrainDeCercleSelectionWindow(player));
          else ((TerrainDeCercleSelectionWindow)value).CreateWindow();

          player.LearnClassSkill(CustomSkill.DruideAssistanceTerrestre);

          break;

        case 5:

          if(player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreAride))
            player.LearnAlwaysPreparedSpell((int)Spell.Fireball, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
            player.LearnAlwaysPreparedSpell(CustomSpell.TempeteDeNeige, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
            player.LearnAlwaysPreparedSpell((int)Spell.LightningBolt, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
            player.LearnAlwaysPreparedSpell((int)Spell.StinkingCloud, CustomClass.Druid);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.DruideEconomieNaturelle);
          player.LearnClassSkill(CustomSkill.DruideRecuperationNaturelle);

          break;

        case 7:
          
          if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreAride))
            player.LearnAlwaysPreparedSpell((int)Spell.Enervation, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
            player.LearnAlwaysPreparedSpell((int)Spell.IceStorm, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
            player.LearnAlwaysPreparedSpell((int)Spell.FreedomOfMovement, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
            player.LearnAlwaysPreparedSpell((int)Spell.PolymorphSelf, CustomClass.Druid);
          
          break;

        case 9:

          if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreAride))
            player.LearnAlwaysPreparedSpell(CustomSpell.MurDePierre, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
            player.LearnAlwaysPreparedSpell((int)Spell.ConeOfCold, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
            player.LearnAlwaysPreparedSpell(CustomSpell.PassageParLesArbres, CustomClass.Druid);
          else if (player.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
            player.LearnAlwaysPreparedSpell(CustomSpell.FleauDinsectes, CustomClass.Druid);

          break;

        case 10: player.LearnClassSkill(CustomSkill.DruideProtectionNaturelle); break;
        case 14: player.LearnClassSkill(CustomSkill.DruideSanctuaireNaturel); break;
      }
    }
  }
}
