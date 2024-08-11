using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AmitieAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oTarget is not NwCreature targetCreature || oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = new();
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      if (nbTargets > 0)
      {
        for (int i = 0; i < nbTargets; i++)
        {
          targets.Add(caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value);
          caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
        }

        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      }
      else
      {
        targets.Add(targetCreature);
      }

      foreach (var targetObject in targets.Distinct())
      {
        if (targetObject is NwCreature target && target.Race.RacialType == RacialType.Animal && target.Master is null 
          && caster.IsReactionTypeHostile(target) && !EffectSystem.IsCharmeImmune(caster, target)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Pacified(), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        }
      }
    }
  }
}
