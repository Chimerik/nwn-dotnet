using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LienTotemElanAuraEffectTag = "_TOTEM_LIEN_ELAN_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterTotemLienElanCallback;
    private static ScriptCallbackHandle onRemoveTotemLienElanCallback;
    
    public static Effect totemLienElanAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.AreaOfEffect((PersistentVfxType)184, onEnterTotemLienElanCallback),
          sprintEffect, disengageEffect, Effect.CutsceneGhost(), Effect.RunAction(onRemovedHandle:onRemoveTotemLienElanCallback));

        eff.Tag = LienTotemElanAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterTotemLienElanAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering == protector
        || entering.Size > CreatureSize.Large)
        return ScriptHandleResult.Handled;

      if(protector.CurrentAction != Action.MoveToPoint)
      {
        EffectUtils.RemoveTaggedEffect(protector, LienTotemElanAuraEffectTag);
        return ScriptHandleResult.Handled;
      }

      SavingThrowFeedback feedback = new();
      int DC = 8 + protector.GetAbilityModifier(Ability.Strength) + NativeUtils.GetCreatureProficiencyBonus(protector);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(entering, Ability.Strength, effectType: SpellEffectType.Knockdown, caster:protector);
      int score = CreatureUtils.GetSkillScore(entering, Ability.Strength, CustomSkill.AthleticsProficiency);
      int roll = CreatureUtils.GetSkillRoll(entering, CustomSkill.AthleticsProficiency, advantage, score, DC);
      int totalSave = roll + score;
      bool saveFailed = totalSave < DC;

      CreatureUtils.SendSkillCheckFeedback(protector, entering, roll, score, advantage, DC, totalSave, saveFailed, "Athlétisme");

      if (saveFailed)
      {
        ApplyKnockdown(entering, CreatureSize.Large, 2);
        NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 12) + protector.GetAbilityModifier(Ability.Strength), DamageType.Bludgeoning)));
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveTotemLienElanAura(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.Commandable = true;

      return ScriptHandleResult.Handled;
    }
  }
}
