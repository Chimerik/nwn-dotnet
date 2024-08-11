using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChanceDebordanteAuraEffectTag = "_CHANCE_DEBORTANTE_AURA_EFFECT";
    public const string ChanceDebordanteEffectTag = "_CHANCE_DEBORDANTE_EFFECT";
    public const string ChanceDebordanteCooldown = "_CHANCE_DEBORDANTE_COOLDOWN";
    public static readonly Native.API.CExoString ChanceDebortanteEffectExoTag = ProtectionStyleEffectTag.ToExoString();
    public static readonly Native.API.CExoString ChanceDebortanteCooldownExo = ChanceDebordanteCooldown.ToExoString();
    private static ScriptCallbackHandle onEnterChanceDebordanteCallback;
    private static ScriptCallbackHandle onExitChanceDebordanteCallback;
    
    public static Effect chanceDebordanteAura(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraOdd, fScale: 2.2f),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterChanceDebordanteCallback, onExitHandle: onExitChanceDebordanteCallback));
      eff.Tag = ChanceDebordanteAuraEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;

      return eff;
    }
    public static Effect chanceDebordante
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)153);
        eff.Tag = ChanceDebordanteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterChanceDebordante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector
        || entering.IsReactionTypeFriendly(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, chanceDebordante));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitChanceDebordante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      foreach (var eff in exiting.ActiveEffects)
        if (eff.Creator == protector && eff.Tag == ChanceDebordanteEffectTag)
          exiting.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
