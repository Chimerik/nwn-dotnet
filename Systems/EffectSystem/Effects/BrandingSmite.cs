using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBrandingSmiteRevealCallback;
    public const string BrandingSmiteAttackEffectTag = "_BRANDING_SMITE_ATTACK_EFFECT";
    public static Effect brandingSmiteAttack
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = BrandingSmiteAttackEffectTag;
        eff.Spell = (Spell)CustomSpell.BrandingSmite;
        return eff;
      }
    }
    public const string BrandingSmiteRevealEffectTag = "_BRANDING_SMITE_REVEAL_EFFECT";
    public static Effect brandingSmiteReveal
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveBrandingSmiteRevealCallback);
        eff.Tag = BrandingSmiteRevealEffectTag;
        eff.Spell = (Spell)CustomSpell.BrandingSmite;
        return eff;
      }
    }
    public static void OnBrandingSmiteReveal(OnStealthModeUpdate onStealth)
    {
      onStealth.EnterOverride = StealthModeOverride.PreventEnter;
      onStealth.Creature?.LoginPlayer.SendServerMessage("Vous ne pouvez pas utiliser la furtivité sous l'effet de la marque de révélation", ColorConstants.Red);
    }
    public static void OnBrandingSmiteReveal(OnEffectApply onEffect)
    {
      switch (onEffect.Effect.EffectType)
      {
        case EffectType.Invisibility:
        case EffectType.ImprovedInvisibility: onEffect.PreventApply = true; break;
      }
    }
    private static ScriptHandleResult OnRemoveBrandingSmite(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnStealthModeUpdate -= OnBrandingSmiteReveal;
      creature.OnEffectApply -= OnBrandingSmiteReveal;

      return ScriptHandleResult.Handled;
    }
  }
}
