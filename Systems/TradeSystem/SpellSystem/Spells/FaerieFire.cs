using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> FaerieFire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      List<NwGameObject> targetList = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, false))
      {
        if (CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          ApplyFaerieFireEffect(oCaster, target, spellEntry);
          targetList.Add(target);
        }
      }

      return targetList;
    }
    public static void ApplyFaerieFireEffect(NwGameObject oCaster, NwCreature target, SpellEntry spellEntry)
    {
      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.faerieFireEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      target.OnEffectApply += EffectSystem.CheckFaerieFire;

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.Invisibility || eff.EffectType == EffectType.ImprovedInvisibility)
          target.RemoveEffect(eff);
    }
  }
}
