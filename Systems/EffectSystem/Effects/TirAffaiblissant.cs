using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveTirAffaiblissantCallback;
    public const string TirAffaiblissantEffectTag = "_TIR_AFFAIBLISSANT_EFFECT";
    public static Effect tirAffaiblissantEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.DamageDecrease), Effect.RunAction(onRemovedHandle: onRemoveTirAffaiblissantCallback));
        eff.Tag = TirAffaiblissantEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveTirAffaiblissant(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwGameObject target)
        return ScriptHandleResult.Handled;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.TirAffaiblissantVariable).Delete();

      return ScriptHandleResult.Handled;
    }
  }
}
