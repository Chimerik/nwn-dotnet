using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EntraveEffectTag = "_ENTRAVE_EFFECT";
    public static readonly Native.API.CExoString EntraveEffectExoTag = EntraveEffectTag.ToExoString();
    private static ScriptCallbackHandle onRemoveEntraveCallback;
    private static ScriptCallbackHandle onIntervalEntraveCallback;
    public static void Entrave(NwCreature target, NwCreature caster, Ability castAbility, TimeSpan duration, bool repeatSave = false)
    {
      target.MovementRate = MovementRate.Immobile;

      Effect eff = repeatSave ? Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEntangle), Effect.RunAction(onRemovedHandle: onRemoveEntraveCallback, onIntervalHandle: onIntervalEntraveCallback, interval: NwTimeSpan.FromRounds(1)))
        : Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEntangle), Effect.RunAction(onRemovedHandle: onRemoveEntraveCallback));

      eff.Tag = EntraveEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.IntParams[5] = (int)castAbility;

      target.ApplyEffect(EffectDuration.Temporary, eff, duration);
    }
    private static ScriptHandleResult OnRemoveEntrave(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        if(target.IsLoginPlayerCharacter)
          target.MovementRate = MovementRate.PC;
        else
          target.MovementRate = MovementRate.CreatureDefault;
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalEntrave(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target && eventData.Effect.Creator is NwCreature caster)
      {
        if(!target.ActiveEffects.Any(e => e.Tag == EffectSystem.TerrainDifficileEffectTag && e.IntParams[5] == CustomSpell.Enchevetrement))
        {
          target.RemoveEffect(eventData.Effect);
          return ScriptHandleResult.Handled;
        }

        int spellDC = SpellUtils.GetCasterSpellDC(caster, (Ability)eventData.Effect.IntParams[5]);

        if (CreatureUtils.GetSavingThrow(caster, target, Ability.Strength, spellDC) != SavingThrowResult.Failure)
          target.RemoveEffect(eventData.Effect);
      }
      else if (eventData.EffectTarget is NwGameObject oTarget)
        oTarget.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}

