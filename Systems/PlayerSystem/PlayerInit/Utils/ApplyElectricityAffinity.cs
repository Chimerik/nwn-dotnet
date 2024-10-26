﻿using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyElectricityAffinity()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteElec)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ElectricityAffinityEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ElectricityAffinity));
      }
    }
  }
}