
using System.Linq;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ExcretionCorrosive(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionAcid));
      
      if (!target.ActiveEffects.Any(e => e.Tag == EffectSystem.ExcretionCorrosiveEffectTag)
        && CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry) == SavingThrowResult.Failure)
      {
        SpellUtils.DealSpellDamage(target, 4, spellEntry, spellEntry.numDice, caster, 2);
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ExcretionCorrosive(1), NwTimeSpan.FromRounds(spellEntry.duration)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
      }
    }
  }
}
