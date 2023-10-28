﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHalfOrcEndurance()
      {
          if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).HasValue
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.enduranceImplacable.Tag))
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.enduranceImplacable);
      }
    }
  }
}
