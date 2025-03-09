using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Sommeil(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> targets = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        TimeSpan duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);
        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);

        foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          if (target == oCaster || !caster.IsReactionTypeHostile(target))
            continue;

          if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry, SpellConfig.SpellEffectType.Sleep) == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Temporary, Effect.Knockdown(), NwTimeSpan.FromRounds(1));
            targets.Add(target);

            EffectSystem.ApplySommeil(target, caster, spell, duration, Ability.Wisdom, castingClass.SpellCastingAbility);
          }
        }
      }

      return targets;
    }
  }
}
