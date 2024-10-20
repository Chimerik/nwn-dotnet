using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VortexDechaineEffectTag = "_VORTEX_DECHAINE_EFFECT";
    public const string VortexDechaineSilenceEffectTag = "_VORTEX_DECHAINE_SILENCE_EFFECT";
    public const string VortexDechaineCooldownTag = "_VORTEX_DECHAINE_COOLDOWN_EFFECT";
    private static ScriptCallbackHandle onEnterVortexDechaineCallback;
    private static ScriptCallbackHandle onExitVortexDechaineCallback;
    private static ScriptCallbackHandle onIntervalVortexDechaineCallback;
    public static Effect VortexDechaine
    {
      get
      { 
        Effect eff = Effect.LinkEffects(Effect.AreaOfEffect((PersistentVfxType)62, onEnterVortexDechaineCallback, onIntervalVortexDechaineCallback,
          onExitVortexDechaineCallback));
        eff.Tag = VortexDechaineEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      } 
    }
    public static Effect VortexDechaineSilence
    {
      get
      {
        Effect eff = Effect.Silence();
        eff.Tag = VortexDechaineSilenceEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect VortexDechaineCooldown
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = VortexDechaineCooldownTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterVortexDechaine(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, VortexDechaineSilence));

      if (!entering.ActiveEffects.Any(e => e.Tag == VortexDechaineCooldownTag && e.Creator == protector))
      {
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
        entering.ApplyEffect(EffectDuration.Temporary, VortexDechaineCooldown, TimeSpan.FromSeconds(5));

        SpellUtils.DealSpellDamage(entering, protector.CasterLevel, Spells2da.spellTable[CustomSpell.VortexDechaine], SpellUtils.GetSpellDamageDiceNumber(protector, NwSpell.FromSpellId(CustomSpell.VortexDechaine)), protector, 3);
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitVortexDechaine(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, VortexDechaineSilenceEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalVortexDechaine(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.VortexDechaine);
      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.VortexDechaine];

      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (target.ActiveEffects.Any(e => e.Tag == VortexDechaineCooldownTag && e.Creator == caster))
          continue;

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
        target.ApplyEffect(EffectDuration.Temporary, VortexDechaineCooldown, TimeSpan.FromSeconds(5));

        SpellUtils.DealSpellDamage(target, 10, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, 3);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
