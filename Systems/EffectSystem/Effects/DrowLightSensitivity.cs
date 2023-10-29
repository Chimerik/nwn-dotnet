using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect lightSensitivity
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(145)));
        eff.Tag = lightSensitivityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string lightSensitivityEffectTag = "_LIGHT_SENSITIVITY_EFFECT";
    public static readonly Native.API.CExoString lightSensitivityEffectExoTag = "_LIGHT_SENSITIVITY_EFFECT".ToExoString();
  }
}
