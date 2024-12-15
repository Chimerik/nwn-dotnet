using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CoupeJarretEffectTag = "_COUPE_JARRET_EFFECT";
    public static void CoupeJarret(NwCreature target)
    {
      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;

      Effect eff = Effect.MovementSpeedDecrease(50);
      eff.Tag = CoupeJarretEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
  }
}

