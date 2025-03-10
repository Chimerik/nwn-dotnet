using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AgresseurSauvageEffectTag = "_AGRESSEUR_SAUVAGE_EFFECT";
    private static ScriptCallbackHandle onRemoveAgresseurSauvageCallback;

    public static void ApplyAgresseurSauvage(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == AgresseurSauvageEffectTag || (e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.AgresseurSauvage)))
        return;

      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.AgresseurSauvage), Effect.RunAction(onRemovedHandle: onRemoveAgresseurSauvageCallback));
      eff.Tag = AgresseurSauvageEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.CasterLevel = CustomSkill.AgresseurSauvage;
      eff.Creator = caster;

      caster.ApplyEffect(EffectDuration.Permanent, eff);
    }
    private static ScriptHandleResult OnRemoveAgresseurSauvage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, CustomSkill.AgresseurSauvage), TimeSpan.FromSeconds(5.9));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
