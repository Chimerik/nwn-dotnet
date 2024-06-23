using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VoeuDHostiliteEffectTag = "_VOEU_D_HOSTILITE_EFFECT";
    public static readonly Native.API.CExoString VoeuDHostiliteEffectExoTag = VoeuDHostiliteEffectTag.ToExoString();
    public static Effect VoeuDHostilite
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = VoeuDHostiliteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
