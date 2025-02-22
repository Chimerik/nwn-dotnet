using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BourrasqueEffectTag = "_BOURRASQUE_EFFECT";
    public const string BourrasqueSlowEffectTag = "_BOURRASQUE_SLOW_EFFECT";
    private static ScriptCallbackHandle onEnterBourrasqueCallback;
    private static ScriptCallbackHandle onExitBourrasqueCallback;
    private static ScriptCallbackHandle onHeartbeatBourrasqueCallback;
    public static Effect Bourrasque(NwCreature caster, NwSpell spell, Ability castingAbility)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.Bourrasque, onEnterBourrasqueCallback, onHeartbeatBourrasqueCallback, onExitBourrasqueCallback);
      eff.Tag = BourrasqueEffectTag;
      eff.Spell = spell;
      eff.Creator = caster;
      return eff;
    }
    public static Effect BourrasqueSlow
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(25);
        eff.Tag = BourrasqueSlowEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterBourrasque(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData))
      {
        if (eventData.Entering is NwCreature entering)
        {
          if (eventData.Effect.Creator is not NwCreature caster)
          {
            eventData.Effect.Destroy();
            return ScriptHandleResult.Handled;
          }

          if (caster.IsReactionTypeHostile(entering))
          {
            if (!entering.ActiveEffects.Any(e => e.Tag == BourrasqueSlowEffectTag))
            {
              NWScript.AssignCommand(caster, () => entering.ApplyEffect(EffectDuration.Permanent, BourrasqueSlow));
              entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
            }

            SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.GustOfWind];
            BourrasqueKnockdown(caster, entering, spellEntry, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);
          }
        }
      }
      else if (eventData.Entering is NwAreaOfEffect aoe)
      {
        switch (aoe.Tag)
        {
          case NappeDeBrouillardEffectTag:
            aoe.Destroy(); break;
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatBourrasque(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData))
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.GustOfWind];

      foreach(NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>()) 
      {
        if (caster.IsReactionTypeHostile(entering))
          BourrasqueKnockdown(caster, entering, spellEntry, (Ability)caster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{eventData.Effect.Spell.Id}").Value);
      }

      foreach (NwAreaOfEffect aoe in eventData.Effect.GetObjectsInEffectArea<NwAreaOfEffect>())
        switch (aoe.Tag)
        {
          case NappeDeBrouillardEffectTag:
            aoe.Destroy(); break;
        }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitBourrasque(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;
      
      EffectUtils.RemoveTaggedEffect(exiting, protector, BourrasqueSlowEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static void BourrasqueKnockdown(NwCreature caster, NwCreature entering, SpellEntry spellEntry, Ability DCAbility)
    {
      int spellDC = SpellUtils.GetCasterSpellDC(caster, DCAbility);

      if (CreatureUtils.GetSavingThrow(caster, entering, Ability.Strength, spellDC) == SavingThrowResult.Failure)
      {
        entering.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(25), NwTimeSpan.FromRounds(1));
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
      }

      entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWind));
    }
  }
}
