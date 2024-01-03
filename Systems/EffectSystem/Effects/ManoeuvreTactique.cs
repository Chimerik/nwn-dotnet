using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManoeuvreTactiqueEffectTag = "_EFFECT_MANOEUVRE_TACTIQUE";
    public static Effect manoeuvreTactique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.Icon(EffectIcon.MovementSpeedIncrease));
        eff.Tag = ManoeuvreTactiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
