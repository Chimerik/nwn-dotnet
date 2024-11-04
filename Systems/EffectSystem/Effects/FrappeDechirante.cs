using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDechiranteEffectTag = "_FRAPPE_DECHIRANTE_EFFECT";
    public static readonly Native.API.CExoString FrappeDechiranteEffectExoTag = "_FRAPPE_DECHIRANTE_EFFECT".ToExoString();
    public static Effect FrappeDechirante
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = FrappeDechiranteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
