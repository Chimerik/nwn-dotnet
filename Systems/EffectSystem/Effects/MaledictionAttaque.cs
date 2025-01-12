using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaledictionAttaqueEffectTag = "_MALEDICTION_ATTAQUE_EFFECT";
    public static Effect MaledictionAttaque
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.Curse);
        eff.Tag = MaledictionAttaqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
