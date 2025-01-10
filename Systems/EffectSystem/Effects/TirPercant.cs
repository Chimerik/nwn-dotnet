using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TirPercantEffectTag = "_TIR_PERCANT_EFFECT";
    public static void TirPercant(NwCreature target)
    {
      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;

      Effect eff = Effect.Icon(CustomEffectIcon.TirPercant);
      eff.Tag = TirPercantEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
  }
}

