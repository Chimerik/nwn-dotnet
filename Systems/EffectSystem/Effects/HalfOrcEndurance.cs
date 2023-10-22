using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect enduranceImplacable;

    public static void InitHalfOrcEnduranceEffect()
    {
      enduranceImplacable = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(146)));
      enduranceImplacable.SubType = EffectSubType.Unyielding;
      enduranceImplacable.Tag = "_HALFORC_ENDURANCE_EFFECT";
    }
  }
}
