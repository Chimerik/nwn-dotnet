using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LoupMorsurePlongeante(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
        
        companion.OnCreatureAttack -= RangerUtils.OnAttackMorsurePlongeante;
        companion.OnCreatureAttack += RangerUtils.OnAttackMorsurePlongeante;

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireLoupMorsurePlongeante, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireLoupMorsurePlongeante, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
