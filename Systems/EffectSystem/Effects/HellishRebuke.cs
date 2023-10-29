using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect hellishRebukeEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(152));
        eff.Tag = HellishRebukeEffectTag;
        return eff;
      }
    }
    public const string HellishRebukeEffectTag = "_HELLISH_REBUKE_EFFECT";
  }
}
