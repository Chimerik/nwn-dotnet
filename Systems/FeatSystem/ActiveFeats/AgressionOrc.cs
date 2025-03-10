﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void AgressionOrc(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || caster == targetObject)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ClearActionQueue();
      _ = caster.AddActionToQueue(() => _ = caster.ActionForceMoveTo(targetObject, true));
      _ = caster.AddActionToQueue(() => _ = caster.ActionAttackTarget(targetObject));

      if(caster.KnowsFeat((Feat)CustomSkill.Chargeur))
        caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.agressionOrc, NwTimeSpan.FromRounds(1));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Agression", ColorConstants.Red, true);

      await NwTask.NextFrame();
      caster.Commandable = false;
    }
  }
}
