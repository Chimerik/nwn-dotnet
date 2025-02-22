using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MagieDeGuerreEffectTag = "_MAGIE_DE_GUERRE_EFFECT";
    public static Effect MagieDeGuerre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.MagieDeGuerre);
        eff.Tag = MagieDeGuerreEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
