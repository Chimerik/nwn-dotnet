using System;
using System.Collections.Generic;
using System.Data;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TirDeBarrage(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      Dictionary<NwGameObject, double> missileDelay = new();

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, spellEntry.aoESize, true, oCaster.Location.Position))
      {
        if (caster.IsReactionTypeHostile(target))
        {
          double targetDistance = oCaster.Distance(target);

          if (!missileDelay.TryAdd(target, 0.1))
            missileDelay[target] += 0.1;

          double delay = (targetDistance / (3.0 * Math.Log(targetDistance) + 2.0)) + missileDelay[target];

          SpellUtils.DelayMirvDamageImpact(oCaster, target, spell, spellEntry, casterClass, delay, nbDices:5, spellDC: spellDC);
          SpellUtils.DelayMirvVisualImpact(oCaster, target, missileDelay[target]);
        }
      }
    }
  }
}
