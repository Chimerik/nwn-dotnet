using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(EffectSystem))]
  public partial class EffectSystem
  {
    private static ScriptHandleFactory scriptHandleFactory;

    public EffectSystem(ScriptHandleFactory scriptFactory)
    {
      scriptHandleFactory = scriptFactory;

      InitThreatenedEffect();
      InitElvenSleepImmunityEffect();
      InitDrowLightSensitivityEffect();
      InitWoodElfSpeedEffect();
      InitHalfOrcEnduranceEffect();
      InitDwarfSlowEffect();
      InitShieldArmorDisadvantageEffect();
      InitHeavyArmorSlowEffect();
    }
  }
}
