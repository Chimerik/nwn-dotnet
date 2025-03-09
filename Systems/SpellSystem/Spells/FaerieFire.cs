using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> FaerieFire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation, NwClass castingClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      Ability castAbility = castingClass.SpellCastingAbility;

      if (oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.FaerieFireDrow)
        castAbility = (Ability)new int[3] { caster.GetAbilityModifier(Ability.Intelligence), caster.GetAbilityModifier(Ability.Wisdom), caster.GetAbilityModifier(Ability.Charisma) }.OrderDescending().First();

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castAbility);

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
