using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TranspercerEffectTag = "_TRANSPERCER_EFFECT";
    public static void Transpercer(NwCreature target)
    {
      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;

      Effect eff = Effect.Icon(CustomEffectIcon.Transpercer);
      eff.Tag = TranspercerEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
  }
}

