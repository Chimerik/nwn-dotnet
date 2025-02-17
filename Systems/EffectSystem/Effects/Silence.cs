using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SilenceAoEEffectTag = "_SILENCE_AOE_EFFECT";
    public const string SilenceEffectTag = "_SILENCE_EFFECT";
    private static ScriptCallbackHandle onEnterSilenceCallback;
    private static ScriptCallbackHandle onExitSilenceCallback;

    public static Effect Silence(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraSilence, fScale: 1.5f),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterSilenceCallback, onExitHandle: onExitSilenceCallback));

      eff.Tag = SilenceAoEEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;

      return eff;
    }
    private static ScriptHandleResult onEnterSilence(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      var eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Sonic, 100), Effect.Deaf());
      eff.Tag = "_SILENCE_EFFECT";
      entering.ApplyEffect(EffectDuration.Permanent, eff);

      entering.OnSpellAction -= OnSpellInputSilenced; 
      entering.OnSpellAction += OnSpellInputSilenced; 

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onExitSilence(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, "_SILENCE_EFFECT");
      exiting.OnSpellAction -= OnSpellInputSilenced;

      return ScriptHandleResult.Handled;
    }

    private static void OnSpellInputSilenced(OnSpellAction onCast)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onCast.Spell.Id];

      if(spellEntry.requiresVerbal)
      {
        onCast.Caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
        onCast.PreventSpellCast = true;
      }
    }
  }
}
