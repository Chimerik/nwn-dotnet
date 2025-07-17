using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SommeilEffectTag = "_SOMMEIL_EFFECT";
    public static readonly Native.API.CExoString SommeilEffectExoTag = SommeilEffectTag.ToExoString();
    private static ScriptCallbackHandle onIntervalSommeilCallback;
    private static ScriptCallbackHandle onRemoveSommeilCallback;
    public static async void ApplySommeil(NwCreature target, NwCreature caster, NwSpell spell, TimeSpan duration, Ability SaveAbility, Ability DCAbility)
    {
      if (spell is not null && spell.SpellType == Spell.Sleep)
      {
        await NwTask.Delay(NwTimeSpan.FromRounds(1));

        if (target is null || !target.IsValid || caster is null || !caster.IsValid)
          return;
      }
      else
        await NwTask.NextFrame();

      if (!IsSleepImmune(target, caster))
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);

        if (CreatureUtils.GetSavingThrowResult(target, SaveAbility, caster, spellDC, effectType: SpellConfig.SpellEffectType.Sleep) == SavingThrowResult.Failure)
        {
         Effect  eff = Effect.LinkEffects(Effect.Sleep(), 
           Effect.RunAction(onRemovedHandle: onRemoveSommeilCallback, onIntervalHandle: onIntervalSommeilCallback, interval: NwTimeSpan.FromRounds(1)));
          eff.Tag = SommeilEffectTag;
          eff.SubType = EffectSubType.Supernatural;
          eff.Creator = caster;
          eff.Spell = spell;
          eff.CasterLevel = (int)DCAbility;

          target.OnDamaged -= OnDamagedSommeil;
          target.OnDamaged += OnDamagedSommeil;

          target.ApplyEffect(EffectDuration.Temporary, eff, duration);
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSleep));
        }
      }
    }
    public static bool IsSleepImmune(NwCreature target, NwCreature caster)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct, RacialType.Elf) || target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 13))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est immunisé au sommeil");
        return true;
      }

      return false;
    }

    private static ScriptHandleResult OnIntervalSommeil(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      var eff = eventData.Effect;

      if (eff.Spell is null && eventData.EffectTarget is NwCreature target && eff.Creator is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)eff.CasterLevel);

        if (CreatureUtils.GetSavingThrowResult(target, Ability.Constitution, caster, spellDC, effectType: SpellConfig.SpellEffectType.Sleep) != SavingThrowResult.Failure)
          EffectUtils.RemoveEffectType(target, EffectType.Sleep);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveSommeil(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
        creature.OnDamaged -= OnDamagedSommeil;

      return ScriptHandleResult.Handled;
    }
    private static void OnDamagedSommeil(CreatureEvents.OnDamaged onDamaged)
    {
      EffectUtils.RemoveEffectType(onDamaged.Creature, EffectType.Sleep);
      EffectUtils.RemoveTaggedEffect(onDamaged.Creature, SommeilEffectTag);
    }
  }
}
