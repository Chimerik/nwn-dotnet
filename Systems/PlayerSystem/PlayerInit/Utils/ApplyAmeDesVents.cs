﻿using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAmeDesVents()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoAmeDesVents)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmeDesVentsEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AmeDesVents));
      }
    }
  }
}
