using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NappeDeBrouillardEffectTag = "_NAPPE_DE_BROUILLARD_EFFECT";
    public const string NappeDeBrouillardBlindEffectTag = "_NAPPE_DE_BROUILLARD_BLIND_EFFECT";
    private static ScriptCallbackHandle onEnterNappeDeBrouillardCallback;
    private static ScriptCallbackHandle onExitNappeDeBrouillardCallback;

    public static Effect NappeDeBrouillard(NwCreature caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.NappeDeBrouillard, onEnterNappeDeBrouillardCallback, onExitHandle: onExitNappeDeBrouillardCallback);
      eff.Tag = NappeDeBrouillardEffectTag;
      eff.Spell = NwSpell.FromSpellId(CustomSpell.NappeDeBrouillard);
      eff.Creator = caster;
      eff.Spell = spell;
      return eff;
    }
    public static Effect NappeDeBrouillardBlind
    {
      get
      {
        Effect eff = Effect.Blindness();
        eff.Tag = NappeDeBrouillardBlindEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterNappeDeBrouillard(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering)
        return ScriptHandleResult.Handled;

      if(eventData.Effect.Creator is not NwCreature caster)
      {
        eventData.Effect.Destroy();
        return ScriptHandleResult.Handled;
      }

      NWScript.AssignCommand(caster, () => entering.ApplyEffect(EffectDuration.Permanent, NappeDeBrouillardBlind));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitNappeDeBrouillard(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, NappeDeBrouillardBlindEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
