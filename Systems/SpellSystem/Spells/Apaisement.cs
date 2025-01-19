using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Apaisement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);
        var spellDuration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

        foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          if (target.HP < 1 || !CreatureUtils.IsHumanoid(target))
            continue;

          if (caster.IsReactionTypeHostile(target))
          {
            if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
            {
              EffectSystem.ApplyCharme(target, caster, spellDuration);
              concentrationList.Add(target);
            }
          }
          else
          {
            EffectUtils.RemoveTaggedEffect(target, EffectSystem.CharmEffectTag, EffectSystem.FrightenedEffectTag);
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.CharmeImmunite, spellDuration);
            target.ApplyEffect(EffectDuration.Temporary, Effect.Immunity(ImmunityType.Fear), spellDuration);
            concentrationList.Add(target);
          }

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
        }
      }

      return concentrationList;
    }
  }
}
