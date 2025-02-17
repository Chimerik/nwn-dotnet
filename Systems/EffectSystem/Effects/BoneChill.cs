using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBoneChillCallback;
    public const string boneChillEffectTag = "_BONE_CHILL_EFFECT";
    public static Effect boneChillEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurBigbysInterposingHand), Effect.RunAction(onRemovedHandle: onRemoveBoneChillCallback));
        eff.Tag = boneChillEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.BoneChill);
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveBoneChill(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwGameObject target)
        return ScriptHandleResult.Handled;

      target.OnHeal -= SpellSystem.PreventHeal;

      return ScriptHandleResult.Handled;
    }
  }
}
