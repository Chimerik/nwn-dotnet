using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PeauDecorceEffectTag = "_PEAU_DECORCE_EFFECT";
    public static readonly Native.API.CExoString peauDecorceEffectExoTag = PeauDecorceEffectTag.ToExoString();
    public static Effect PeauDecorce
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACIncrease);
        eff.Tag = PeauDecorceEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
