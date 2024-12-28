using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Benediction(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      List<NwGameObject> objectTargets = new();

      if (oCaster is not NwCreature caster)
        return objectTargets;

      List<NwGameObject> targets = new();

      if (caster.IsPlayerControlled)
      {
        targets.AddRange(SpellUtils.GetSpellTargets(caster, caster, spellEntry, true));
      }
      else
      {
        foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
          if(caster.IsReactionTypeFriendly(target))
            targets.Add(target);
      }
      
      SpellUtils.SignalEventSpellCast(caster, caster, Spell.Bless);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (var targetObject in targets)
      {
        objectTargets.Add(targetObject);
        targetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));
        NWScript.AssignCommand(caster, () => targetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.Benediction, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      }

      return objectTargets;
    }
  }
}
