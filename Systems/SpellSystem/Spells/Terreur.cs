using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Terreur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation)
    {
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 9, false, oCaster.Location.Position))
      {
        if (EffectSystem.IsFrightImmune(target, caster))
          continue;

        if (CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetTerreurEffect(caster, target, casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          targetList.Add(target);
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFearS));
        }
      }

      return targetList;
    }
  }
}
