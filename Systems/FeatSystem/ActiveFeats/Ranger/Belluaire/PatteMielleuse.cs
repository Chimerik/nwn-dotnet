using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PatteMielleuse(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
        
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
