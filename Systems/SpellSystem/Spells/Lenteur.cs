using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Lenteur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is not NwCreature caster)
        return concentrationTargets;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, false))
      {
        if (!caster.IsReactionTypeHostile(target) 
          || CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) != SavingThrowResult.Failure)
          continue;

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Lenteur(castingClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
        concentrationTargets.Add(target);
      }

      return concentrationTargets;
    }
  }
}
