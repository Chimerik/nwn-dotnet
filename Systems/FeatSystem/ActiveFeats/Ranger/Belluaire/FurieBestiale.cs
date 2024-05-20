using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FurieBestiale(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        if (companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeCoolDownVariable).HasValue)
          caster.LoginPlayer?.SendServerMessage("aucune utilisation restante", ColorConstants.Red);
        else
        {
          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
          companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeCoolDownVariable).Value = 1;
          companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeVariable).Value = 1;
        }
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
