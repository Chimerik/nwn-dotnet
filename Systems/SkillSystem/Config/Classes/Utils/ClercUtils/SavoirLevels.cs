using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleSavoirLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine du Savoir");
          player.oid.SetTextureOverride("clerc", "domaine_savoir");

          List<int> tempList = new() { CustomSkill.ArcanaProficiency, CustomSkill.HistoryProficiency, CustomSkill.NatureProficiency, CustomSkill.ReligionProficiency };
          List<int> skillList = new();

          foreach(var skill in tempList)
            if(!player.learnableSkills.TryGetValue(skill + 1, out var expertise) || expertise.currentLevel < 1)
              skillList.Add(skill);

          player.LearnClassSkill(CustomSkill.ClercSavoirAncestral);

          if (!player.windows.TryGetValue("skillProficiencySelection", out var skill3)) player.windows.Add("skillProficiencySelection", new SkillProficiencySelectionWindow(player, skillList, 2, CustomSkill.ClercSavoir));
          else ((SkillProficiencySelectionWindow)skill3).CreateWindow(skillList, 2, CustomSkill.ClercSavoir);

          player.LearnAlwaysPreparedSpell(CustomSpell.Injonction, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Identify, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Augure, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.HoldPerson, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.CommunicationAvecLesMorts, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Antidetection, CustomClass.Clerc);

          break;

        case 6: player.LearnClassSkill(CustomSkill.ClercDetectionDesPensees); break;

        case 7:

          player.LearnAlwaysPreparedSpell(CustomSpell.OeilMagique, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Confusion, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.Scrutation, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.LegendLore, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercHaloDeLumiere); break;
      }
    }
  }
}
