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

          if (!player.windows.TryGetValue("expertiseChoice", out var skill3)) player.windows.Add("expertiseChoice", new ExpertiseChoiceWindow(player));
          else ((ExpertiseChoiceWindow)skill3).CreateWindow();

          player.LearnClassSkill(CustomSkill.ClercSavoirAncestral);
          player.LearnAlwaysPreparedSpell(CustomSpell.Injonction, CustomClass.Clerc);
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

        case 17: player.LearnClassSkill(CustomSkill.ClercVisionDuPasse); break;
      }
    }
  }
}
