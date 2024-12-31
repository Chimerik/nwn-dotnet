using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleNatureLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Nature");
          player.oid.SetTextureOverride("clerc", "nature_domain");

          List<NwSpell> druidCantrips = new() { NwSpell.FromSpellId(CustomSpell.Shillelagh), NwSpell.FromSpellId(CustomSpell.Elementalisme),
            NwSpell.FromSpellType(Spell.RayOfFrost), NwSpell.FromSpellId(CustomSpell.Message), NwSpell.FromSpellType(Spell.ElectricJolt),
          NwSpell.FromSpellType(Spell.GreatThunderclap), NwSpell.FromSpellId(CustomSpell.FireBolt), NwSpell.FromSpellId(CustomSpell.PoisonSpray),
          NwSpell.FromSpellId(CustomSpell.ProduceFlame), NwSpell.FromSpellId(CustomSpell.Druidisme) };

          if (!player.windows.TryGetValue("cantripSelection", out var cantrip1)) player.windows.Add("cantripSelection", new CantripSelectionWindow(player, ClassType.Druid, 1, druidCantrips));
          else ((CantripSelectionWindow)cantrip1).CreateWindow(ClassType.Druid, 1, druidCantrips);

          List<int> skillList = new() { CustomSkill.AnimalHandlingProficiency, CustomSkill.NatureProficiency, CustomSkill.SurvivalProficiency };

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 1));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 1);

          player.LearnClassSkill(CustomSkill.ClercCharmePlanteEtAnimaux);

          player.LearnAlwaysPreparedSpell((int)Spell.CharmPersonOrAnimal, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.SpeakAnimal, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceDepines, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Barkskin, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.TempeteDeNeige, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceVegetale, CustomClass.Clerc);

          break;

        case 6: player.LearnClassSkill(CustomSkill.ClercAttenuationElementaire); break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.LianeAvide, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.DominateAnimal, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.MurDePierre, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.FleauDinsectes, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercMaitreDeLaNature); break;
      }
    }
  }
}
