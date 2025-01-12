using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeGuideeEffectTag = "_FRAPPE_GUIDEE_EFFECT";
    public static Effect FrappeGuidee
    {
      get
      {        
        Effect eff = Effect.Icon(CustomEffectIcon.FrappeGuidee);
        eff.Tag = FrappeGuideeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
