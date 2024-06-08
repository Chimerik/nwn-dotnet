using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string WolAspectAuraEffectTag = "_WOLF_ASPECT_AURA_EFFECT";
    public const string WolfAspectEffectTag = "_WOLF_ASPECT_EFFECT";
    private static ScriptCallbackHandle onEnterWolfAspectAuraCallback;
    private static ScriptCallbackHandle onExitWolfAspectAuraCallback;
    
    public static Effect wolfAspectAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)193, onEnterHandle: onEnterWolfAspectAuraCallback, onExitHandle: onExitWolfAspectAuraCallback);
        eff.Tag = WolAspectAuraEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public static Effect GetWolfAspectEffect(int dexterityModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.SkillIncrease(Skill.MoveSilently, dexterityModifier), Effect.Icon(EffectIcon.SkillIncrease));
      eff.Tag = WolfAspectEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterWolfAspectAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.GetAbilityModifier(Ability.Dexterity) < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, GetWolfAspectEffect(entering.GetAbilityModifier(Ability.Dexterity))));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWolfAspectAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      foreach (var eff in exiting.ActiveEffects)
        if (eff.Creator == protector && eff.Tag == WolfAspectEffectTag)
          exiting.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
