using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChargeDuSanglierAuraEffectTag = "_CHARGE_DU_SANGLIER_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterChargeDuSanglierCallback;
    private static ScriptCallbackHandle onRemoveChargeDuSanglierCallback;
    
    public static Effect ChargeDuSanglierAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.AreaOfEffect((PersistentVfxType)184, onEnterChargeDuSanglierCallback),
          sprintEffect, disengageEffect, Effect.CutsceneGhost(), Effect.RunAction(onRemovedHandle: onRemoveChargeDuSanglierCallback));

        eff.Tag = ChargeDuSanglierAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnEnterChargeDuSanglierAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering == protector
        || entering.Size > CreatureSize.Large)
        return ScriptHandleResult.Handled;

      if (protector.CurrentAction != Action.MoveToPoint)
      {
        EffectUtils.RemoveTaggedEffect(protector, ChargeDuSanglierAuraEffectTag);
        return ScriptHandleResult.Handled;
      }

      NwCreature master = protector.Master is null ? protector : protector.Master;

      int DC = 8 + protector.GetAbilityModifier(Ability.Strength) + NativeUtils.GetCreatureProficiencyBonus(master);
      int targetRoll = entering.GetAbilityModifier(Ability.Strength) + Utils.RollAdvantage(CreatureUtils.GetCreatureAbilityAdvantage(entering, Ability.Strength, effectType: SpellEffectType.Knockdown, caster: protector));

      if (PlayerSystem.Players.TryGetValue(entering, out PlayerSystem.Player player) &&
        player.learnableSkills.TryGetValue(CustomSkill.StrengthSavesProficiency, out LearnableSkill strSave) && strSave.currentLevel > 0)
        targetRoll += NativeUtils.GetCreatureProficiencyBonus(entering);

      if (targetRoll > DC)
        return ScriptHandleResult.Handled;

      ApplyKnockdown(entering, protector);
      
      if (protector.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph && e.IntParams[0] == 108))
        NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 6, 2) + protector.GetAbilityModifier(Ability.Strength), DamageType.Piercing)));
      else
        NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 4) + protector.GetAbilityModifier(Ability.Strength), DamageType.Slashing)));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveChargeDuSanglierAura(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.Commandable = true;

      return ScriptHandleResult.Handled;
    }
  }
}
