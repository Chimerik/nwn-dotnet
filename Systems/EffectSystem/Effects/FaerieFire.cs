using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveFaerieFireCallback;
    public const string faerieFireEffectTag = "_FAERIE_FIRE_EFFECT";
    public static Effect faerieFireEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurGlowLightBlue), Effect.RunAction(onRemovedHandle: onRemoveFaerieFireCallback));
        eff.Tag = faerieFireEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.FaerieFire);
        return eff;
      }
    }
    public static void CheckFaerieFire(OnEffectApply onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Effect.Tag != boneChillEffectTag || onEffect.Object is not NwGameObject target)
        return;

      if (onEffect.Effect.EffectType != EffectType.Invisibility && onEffect.Effect.EffectType != EffectType.ImprovedInvisibility)
        return;

      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == faerieFireEffectTag)
        {
          onEffect.PreventApply = true;
          return;
        }
    }
    private static ScriptHandleResult OnRemoveFaerieFire(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwGameObject target)
        return ScriptHandleResult.Handled;

      target.OnEffectApply -= CheckFaerieFire;

      return ScriptHandleResult.Handled;
    }
  }
}
