using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BrandingSmiteAttackEffectTag = "_BRANDING_SMITE_ATTACK_EFFECT";
    public static Effect brandingSmiteAttack
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = BrandingSmiteAttackEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.BrandingSmite);
        return eff;
      }
    }
    public const string BrandingSmiteRevealEffectTag = "_BRANDING_SMITE_REVEAL_EFFECT";
    public static Effect brandingSmiteReveal
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = BrandingSmiteRevealEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.BrandingSmite);
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
    public static void OnBrandingSmiteReveal(OnEffectRemove onEffect)
    {
      if (onEffect.Object is not NwCreature creature)
        return;

      creature.OnStealthModeUpdate -= OnBrandingSmiteReveal;
      creature.OnEffectApply -= OnBrandingSmiteReveal;
      creature.OnEffectRemove -= OnBrandingSmiteReveal;
    }
  }
}
