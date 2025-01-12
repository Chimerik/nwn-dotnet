using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CeciteSurditeEffectTag = "_CECITE_SURDITE_EFFECT";
    public static Effect CeciteSurdite
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Blindness(), Effect.Deaf());
        eff.Tag = CeciteSurditeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

