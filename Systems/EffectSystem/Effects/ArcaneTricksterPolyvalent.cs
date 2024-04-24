using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ArcaneTricksterPolyvalentEffectTag = "_POLYVALENT_TRICKSTER_EFFECT";
    public static readonly Native.API.CExoString arcaneTricksterPolyvalentEffectExoTag = ArcaneTricksterPolyvalentEffectTag.ToExoString();
    public static Effect ArcaneTricksterPolyvalent
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = ArcaneTricksterPolyvalentEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
