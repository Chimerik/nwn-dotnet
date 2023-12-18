using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AgressionOrc(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
      {
        caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
      _ = caster.ActionAttackTarget(targetCreature);
      caster.Commandable = false;
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.agressionOrc, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Agression", ColorConstants.Red, true);
    }
  }
}
