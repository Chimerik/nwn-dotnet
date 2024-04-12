using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HellishRebukeSourceEffectTag = "_HELLISH_REBUKE_SOURCE_EFFECT";
    public static Effect hellishRebukeSourceEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(152));
        eff.Tag = HellishRebukeSourceEffectTag;
        return eff;
      }
    }
    public const string HellishRebukeTargetTag = "_HELLISH_REBUKE_TARGET_EFFECT";
    public static Effect hellishRebukeTargetEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(152));
        eff.Tag = HellishRebukeTargetTag;
        return eff;
      }
    }
  }
}
