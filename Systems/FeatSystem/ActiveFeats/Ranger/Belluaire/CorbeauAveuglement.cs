using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CorbeauAveuglement(NwCreature caster)
    {
      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is not null)
      {      
        companion.OnCreatureAttack -= RangerUtils.OnAttackCorbeauAveuglement;
        companion.OnCreatureAttack += RangerUtils.OnAttackCorbeauAveuglement;

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauAveuglement, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauAveuglement, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
