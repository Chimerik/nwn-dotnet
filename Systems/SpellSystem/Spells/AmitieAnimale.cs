using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AmitieAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oTarget is not NwCreature target || oCaster is not NwCreature caster)
        return;

      int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);

      if (target.Race.RacialType == RacialType.Animal && target.Master is null && caster.IsReactionTypeHostile(target) && !EffectSystem.IsCharmeImmune(caster, target)
          && !CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC))
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Pacified(), NwTimeSpan.FromRounds(spellEntry.duration)));
      }
    }
  }
}
