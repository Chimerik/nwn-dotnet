using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MetamagieEffectTag = "_METAMAGIE_EFFECT";
    public static Effect MetaMagie(int type)
    {
      Effect eff = Effect.VisualEffect(VfxType.DurIounstoneBlue);
      eff.IntParams[5] = type;
      eff.Tag = MetamagieEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
