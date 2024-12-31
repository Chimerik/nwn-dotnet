using Anvil.API;
using NWN.Core;
using NWN.Native.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProjectileMagique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry);

      Dictionary<NwGameObject, double> missileDelay = new();
      
      foreach (var target in targets)
      {
        double targetDistance = oCaster.Distance(target);

        if (!missileDelay.TryAdd(target, 0.1))
          missileDelay[target] += 0.1;

        double delay = (targetDistance / (3.0 * Math.Log(targetDistance) + 2.0)) + missileDelay[target];

        SpellUtils.DelayMirvDamageImpact(oCaster, target, spell, spellEntry, casterClass, delay);
        SpellUtils.DelayMirvVisualImpact(oCaster, target, missileDelay[target]);
      }
    }
  }
}
