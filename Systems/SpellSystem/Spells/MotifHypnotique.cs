using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MotifHypnotique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentration = new();

      if (oCaster is not NwCreature caster)
        return concentration;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      var duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, true))
      {
        if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          EffectSystem.ApplyCharme(target, caster, spell, duration, false, CustomSpell.MotifHypnotique);
          concentration.Add(target);
        }
      }

      return concentration;
    }
  }
}
