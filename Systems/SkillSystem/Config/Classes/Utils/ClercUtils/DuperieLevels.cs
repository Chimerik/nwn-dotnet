using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleDuperieLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Duperie");
          player.oid.SetTextureOverride("clerc", "duperie");

          player.LearnClassSkill(CustomSkill.ClercBenedictionDuFilou);
          player.LearnClassSkill(CustomSkill.ClercRepliqueInvoquee);

          player.LearnAlwaysPreparedSpell(CustomSpell.Deguisement, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.CharmPerson, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.PassageSansTrace, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Invisibility, CustomClass.Clerc);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.Fear, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Antidetection, CustomClass.Clerc);

          break;


        case 6: player.LearnClassSkill(CustomSkill.TeleportRepliqueDuplicite); break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.Confusion, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.PorteDimensionnelle, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.DominatePerson, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.AlterationMemorielle, CustomClass.Clerc);

          break;
      }
    }
  }
}
