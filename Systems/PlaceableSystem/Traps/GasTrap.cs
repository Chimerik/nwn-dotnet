using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void GasTrap(NwGameObject trap, TrapEntry entry)
    {
      trap.Location.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerFogacid, gasTrapHandler), TimeSpan.FromSeconds(entry.duration));
      UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().GetObjectVariable<LocalVariableInt>("_GAS_TRAP_TYPE").Value = entry.baseDC;
    }
    private ScriptHandleResult OnEnterGasTrap(CallInfo _)
    {
      if(_.ObjectSelf is not NwAreaOfEffect aoe || NWScript.GetEnteringObject().ToNwObject<NwGameObject>() is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.ApplyEffect(EffectDuration.Instant, Effect.Poison((PoisonType)aoe.GetObjectVariable<LocalVariableInt>("_GAS_TRAP_TYPE").Value));
      return ScriptHandleResult.Handled;
    }
  }
}
