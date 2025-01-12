using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FlecheAcideDeMelfEffectTag = "_FLECHE_ACIDE_DE_MELF_EFFECT";
    private static ScriptCallbackHandle onRemoveFlecheAcideDeMelfCallback;
    public static Effect FlecheAcideDeMelf
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveFlecheAcideDeMelfCallback);
        eff.Tag = FlecheAcideDeMelfEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveFlecheAcideDeMelf(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
        creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(4, 2), DamageType.Acid));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
