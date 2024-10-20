using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TrancheVueEffectTag = "_TRANCHE_VUE_EFFECT";
    private static ScriptCallbackHandle onRemoveTrancheVueCallback;
    public static Effect TrancheVue(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)187), Effect.RunAction(onRemovedHandle: onRemoveTrancheVueCallback));
      eff.Tag = TrancheVueEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      caster.OnCreatureAttack -= OnAttackTrancheVue;
      caster.OnCreatureAttack += OnAttackTrancheVue;

      return eff;
    }
    public static void OnAttackTrancheVue(OnCreatureAttack onAttack)
    {
      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurBlindvision), Effect.Blindness()), NwTimeSpan.FromRounds(2));
          onAttack.Attacker.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBlindDeafM));
          
          break;
      }

      EffectUtils.RemoveTaggedEffect(onAttack.Attacker, TrancheVueEffectTag);
    }
    private static ScriptHandleResult OnRemoveTrancheVue(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        caster.OnCreatureAttack -= OnAttackTrancheVue;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
