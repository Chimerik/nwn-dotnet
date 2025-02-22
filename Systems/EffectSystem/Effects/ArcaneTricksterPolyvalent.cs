using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ArcaneTricksterPolyvalentEffectTag = "_POLYVALENT_TRICKSTER_EFFECT";
    public static Effect ArcaneTricksterPolyvalent
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurBigbysInterposingHand, fScale: 0.5f); // TODO : est-ce qu'il existe la même main en bleu clair dans les VFX ?
        eff.Tag = ArcaneTricksterPolyvalentEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
