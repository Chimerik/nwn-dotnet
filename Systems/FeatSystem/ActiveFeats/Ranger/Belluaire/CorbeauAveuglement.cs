using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CorbeauAveuglement(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
        
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
