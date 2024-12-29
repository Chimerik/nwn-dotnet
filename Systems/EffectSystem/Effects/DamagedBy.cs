using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DamagedByEffectTag = "_DAMAGED_BY_EFFECT";
    public static Effect DamagedBy
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = DamagedByEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
