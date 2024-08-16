using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Fleau(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      List<NwGameObject> objectTargets = new();

      if (oCaster is not NwCreature caster)
        return objectTargets;

      List<NwGameObject> targets = new();

      if (caster.IsLoginPlayerCharacter)
      {
        targets.AddRange(SpellUtils.GetSpellTargets(caster, caster, spellEntry, true));
      }
      else
      {
        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
          if(caster.IsReactionTypeHostile(target))
            targets.Add(target);
      }
      
      SpellUtils.SignalEventSpellCast(caster, caster, Spell.Bane);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosEvil10));

      foreach (var targetObject in targets)
      {
        if (targetObject is NwCreature target
          && CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          objectTargets.Add(targetObject);
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Fleau, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        }
      }

      return objectTargets;
    }
  }
}
