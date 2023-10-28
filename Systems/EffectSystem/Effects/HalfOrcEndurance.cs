using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect enduranceImplacable
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(146)));
        eff.Tag = EnduranceImplacableEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string EnduranceImplacableEffectTag = "_HALFORC_ENDURANCE_EFFECT";
    public const string EnduranceImplacableVariable = "_HALFORC_ENDURANCE";
  }
}
