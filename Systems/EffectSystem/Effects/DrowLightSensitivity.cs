using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect lightSensitivity;
    public static readonly string lightSensitivityEffectTag = "_LIGHT_SENSITIVITY_EFFECT";
    public static readonly Native.API.CExoString lightSensitivityEffectExoTag = "_LIGHT_SENSITIVITY_EFFECT".ToExoString();

    public static void InitDrowLightSensitivityEffect()
    {
      lightSensitivity = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(145)));
      lightSensitivity.SubType = EffectSubType.Unyielding;
      lightSensitivity.Tag = lightSensitivityEffectTag;
    }
  }
}
