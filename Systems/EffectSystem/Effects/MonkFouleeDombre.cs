using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FouleeDombreEffectTag = "_FOULEE_DOMBRE_EFFECT";
    public static Effect FouleeDombre
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = FouleeDombreEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
