using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalSpiderCocoonCallback;
    public const string SpiderCocoonEffectTag = "_SPIDER_COCOON_EFFECT";
    public static Effect SpiderCocoon
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Paralyze), Effect.CutsceneParalyze(), Effect.RunAction(onIntervalHandle: onIntervalSpiderCocoonCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = RegardHypnotiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalSpiderCocoon(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      
      if (CreatureUtils.GetSavingThrow(caster, target, Ability.Strength, 8 + NativeUtils.GetCreatureProficiencyBonus(caster.Master) + caster.GetAbilityModifier(Ability.Wisdom)))
      {
        target.RemoveEffect(eventData.Effect);
        target.OnDamaged -= OnDamageSpiderCocoon;
        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
    public static void OnDamageSpiderCocoon(CreatureEvents.OnDamaged onDamage)
    {
      EffectUtils.RemoveTaggedEffect(onDamage.Creature, SpiderCocoonEffectTag);
    }
  }
}
