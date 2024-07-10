using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> DominationAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is NwCreature caster && oTarget is NwCreature target)
      {

        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(castingClass.SpellCastingAbility);

        if (target.Race.RacialType == RacialType.Animal && target.Master is null && !EffectSystem.IsCharmeImmune(caster, target)
            && !CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC))
        {
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Dominated(), NwTimeSpan.FromRounds(spellEntry.duration)));
          return new List<NwGameObject>() { oTarget };
        }
      }

      return new List<NwGameObject>();
    }
  }
}
