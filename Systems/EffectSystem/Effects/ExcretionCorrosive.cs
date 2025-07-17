using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalExcretionCorrosiveCallback;
    public const string ExcretionCorrosiveEffectTag = "_EXCRETION_CORROSIVE_EFFECT";
    public static Effect ExcretionCorrosive(int corrosionLevel)
    {
      Effect eff;

      if (corrosionLevel < 4)
      {
        eff = Effect.LinkEffects(Effect.ACDecrease(corrosionLevel, ACBonus.Natural),
          Effect.RunAction(onIntervalHandle: onIntervalExcretionCorrosiveCallback, interval: NwTimeSpan.FromRounds(1)));
      }
      else
        eff = Effect.ACDecrease(corrosionLevel, ACBonus.Natural);

      eff.Tag = ExcretionCorrosiveEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = corrosionLevel;
      return eff;
    }
    private static ScriptHandleResult OnIntervalExcretionCorrosive(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.ExcretionCorrosive];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.ExcretionCorrosive), Ability.Wisdom);
      int ACMalus = eventData.Effect.CasterLevel + 1;

      EffectUtils.RemoveTaggedEffect(target, ExcretionCorrosiveEffectTag);

      if (CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry) == SavingThrowResult.Failure)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 10), DamageType.Acid)));
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, ExcretionCorrosive(ACMalus), NwTimeSpan.FromRounds(spellEntry.duration)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
