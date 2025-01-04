using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FurieBestiale(NwCreature caster)
    {
      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is not null)
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 6, CustomSkill.BelluaireFurieBestiale));
        companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeVariable).Value = 1;
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
