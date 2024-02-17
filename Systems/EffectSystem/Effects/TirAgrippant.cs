using System.Linq;
using System.Numerics;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalTirAgrippantCallback;
    public const string TirAgrippantTag = "_TIR_AGRIPPANT_EFFECT";
    public static Effect tirAgrippantEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurWeb), Effect.RunAction(onIntervalHandle: onIntervalTirAgrippantCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = boneChillEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalTirAgrippant(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature attacker)
        return ScriptHandleResult.Handled;

      if(Vector3.Distance(target.GetObjectVariable<LocalVariableLocation>(CreatureUtils.TirAgrippantVariable).Value.Position,
        target.Position) > 0.3)
      {
        target.GetObjectVariable<LocalVariableLocation>(CreatureUtils.TirAgrippantVariable).Value = target.Location;

        SpellConfig.SavingThrowFeedback feedback = new();
        int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(attacker) + attacker.GetAbilityModifier(Ability.Intelligence);
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Strength);
        int roll = Utils.RollAdvantage(advantage);
        int score = CreatureUtils.GetSkillScore(target, Ability.Strength, CustomSkill.AthleticsProficiency);
        int totalSave = roll + score;
        bool saveFailed = totalSave <= tirDC;

        CreatureUtils.SendSkillCheckFeedback(attacker, target, roll, score, advantage, tirDC, totalSave, saveFailed, "Athlétisme");

        if (saveFailed)
        {
          int damage = attacker.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
            ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

          NWScript.AssignCommand(attacker, () => target.ApplyEffect(EffectDuration.Instant,
            Effect.Damage(damage, DamageType.Slashing)));

          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Effet - Tir Aggripant", ColorConstants.Red, true);
        }
        else
        {
          target.GetObjectVariable<LocalVariableLocation>(CreatureUtils.TirAgrippantVariable).Delete();
          EffectUtils.RemoveTaggedEffect(target, TirAgrippantTag);
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
