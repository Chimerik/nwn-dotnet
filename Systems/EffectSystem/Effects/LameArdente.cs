using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LameArdenteEffectTag = "_LAME_ARDENTE_EFFECT";
    public static Effect LameArdente
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.LameArdente);
        eff.Tag = LameArdenteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
