﻿using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void FormeSauvage(NwCreature caster, int featId, byte formeSauvageCharges = 1)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      var previousPolymorph = caster.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.Polymorph);

      if (previousPolymorph is not null)
      {
        caster.RemoveEffect(previousPolymorph);

        await NwTask.Delay(TimeSpan.FromSeconds(1.1));

        if (caster is null || !caster.IsValid)
          return;
      }

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(featId))));
      DruideUtils.DecrementFormeSauvage(caster, formeSauvageCharges);
    }
  }
}
