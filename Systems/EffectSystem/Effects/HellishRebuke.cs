using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HellishRebukeEffectTag = "_HELLISH_REBUKE_EFFECT";
    public static Effect hellishRebukeEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(152));
        eff.Tag = HellishRebukeEffectTag;
        return eff;
      }
    }
  }
}
