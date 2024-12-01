using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleArchifeeLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Archifée");
          player.oid.SetTextureOverride("occultiste", "warlock_archfey");

          player.LearnAlwaysPreparedSpell(CustomSpell.Apaisement, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.FaerieFire, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.FouleeBrumeuse, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ForceFantasmagorique, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.Sleep, CustomClass.Occultiste);


          player.LearnClassSkill(CustomSkill.FouleeRafraichissante);
          player.LearnClassSkill(CustomSkill.FouleeProvocatrice);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.Clignotement, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceVegetale, CustomClass.Occultiste);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.FouleeEvanescente);
          player.LearnClassSkill(CustomSkill.FouleeRedoutable);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.ImprovedInvisibility, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.DominateAnimal, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.DominatePerson, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ApparencesTrompeuses, CustomClass.Occultiste);

          break;

        case 10: player.LearnClassSkill(CustomSkill.DefensesEnjoleuses); break;

        case 14: player.LearnClassSkill(CustomSkill.FouleeEnjoleuse); break;
      }
    }
  }
}
