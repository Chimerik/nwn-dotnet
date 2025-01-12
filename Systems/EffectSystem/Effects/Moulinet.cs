using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MoulinetEffectTag = "_MOULINET_EFFECT";
    public static Effect Moulinet
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.ACDecrease));
        eff.Tag = MoulinetEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

