using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string WildMagicBienfaitEffectTag = "_WILD_MAGIC_BIENFAIT_EFFECT";
    public static Effect wildMagicBienfait
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = WildMagicBienfaitEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
