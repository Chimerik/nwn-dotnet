using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EntailleEffectTag = "_ENTAILLE_EFFECT";
    public static readonly Native.API.CExoString EntailleEffectExoTag = EntailleEffectTag.ToExoString();
    public static Effect Entaille
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.Entaille);
        eff.Tag = EntailleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

