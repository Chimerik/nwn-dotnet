using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RegardHypnotique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || EffectSystem.IsCharmeImmune(caster, target))
        return concentrationTargets;

      if (caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).HasValue && 
        caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value.Split("_").Contains(target.ToString()))
      {
        caster.LoginPlayer?.SendServerMessage("Cette capacité n'aura plus d'effet sur cette créature jusqu'à votre prochain repos long", ColorConstants.Red);
        return concentrationTargets;
      }

      SpellUtils.SignalEventSpellCast(oTarget, caster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);
      caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value += $"{caster}_";

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, effectType: SpellConfig.SpellEffectType.Charm) == SavingThrowResult.Failure)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.RegardHypnotique));

        target.OnDamaged -= EffectSystem.OnDamageRegardHypnotique;
        target.OnDamaged += EffectSystem.OnDamageRegardHypnotique;

        concentrationTargets.Add(target);
      }

      return concentrationTargets;
    }
  }
}
