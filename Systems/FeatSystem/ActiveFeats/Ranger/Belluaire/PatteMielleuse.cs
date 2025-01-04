using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PatteMielleuse(NwCreature caster)
    {
      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);
      if (companion is not null)
      {       
        companion.OnCreatureAttack -= RangerUtils.OnAttackPatteMielleuse;
        companion.OnCreatureAttack += RangerUtils.OnAttackPatteMielleuse;

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluairePatteMielleuse, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
