using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CorbeauMauvaisAugure(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is not null)
      {
        _ = companion.ActionCastSpellAt((Spell)CustomSpell.MauvaisAugure, target, cheat:true);

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
