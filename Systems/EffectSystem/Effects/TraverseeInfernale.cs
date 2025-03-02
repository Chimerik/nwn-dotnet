using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TraverseeInfernaleBuffEffectTag = "_TRAVERSEE_INFERNALE_BUFF_EFFECT";
    public const string TraverseeInfernaleEffectTag = "_TRAVERSEE_INFERNALE_EFFECT";
    private static ScriptCallbackHandle onRemoveTraverseeInfernaleCallback;
    public static Effect TraverseeInfernaleBuff(NwCreature caster)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.TraverseeInfernale);
      eff.Tag = TraverseeInfernaleBuffEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      caster.OnCreatureAttack -= OccultisteUtils.OnAttackTraverseeInfernale;
      caster.OnCreatureAttack += OccultisteUtils.OnAttackTraverseeInfernale;

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.TraverseeInfernale);

      return eff;
    }
    public static Effect TraverseeInfernale(NwCreature target)
    {
      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveTraverseeInfernaleCallback);
      eff.Tag = TraverseeInfernaleEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - Traversée  Infernale", ColorConstants.Red, true, true);

      target.ClearActionQueue();
      target.Commandable = false;
      target.VisibilityOverride = VisibilityMode.Hidden;
      target.PlotFlag = true;

      return eff;
    }
    private static ScriptHandleResult onRemoveTraverseeInfernale(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.Commandable = true;
        target.VisibilityOverride = VisibilityMode.Default;
        target.PlotFlag = false;

        if(target.Race.RacialType != CustomRacialType.Fielon && eventData.Effect.Creator is NwCreature caster)
        {
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 10, 8), CustomDamageType.Psychic)));
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}

