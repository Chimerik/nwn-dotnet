using System;
using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void AgressionOrc(NwCreature caster, NwGameObject targetObject)
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
      _ = caster.ClearActionQueue();
      _ = caster.AddActionToQueue(() => caster.ActionForceMoveTo(targetObject, true));
      _ = caster.AddActionToQueue(() => caster.ActionAttackTarget(targetObject));

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.agressionOrc, NwTimeSpan.FromRounds(1));

      CreatureUtils.HandleBonusActionCooldown(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Agression", ColorConstants.Red, true);

      await NwTask.NextFrame();
      caster.Commandable = false;
    }
  }
}
