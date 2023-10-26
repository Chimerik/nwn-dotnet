using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect sleepImmunity;

    public static void InitElvenSleepImmunityEffect()
    {
      sleepImmunity = Effect.Immunity(ImmunityType.Sleep);
      sleepImmunity.ShowIcon = false;
      sleepImmunity.SubType = EffectSubType.Unyielding;
      sleepImmunity.Tag = "_ELVEN_SLEEP_IMMUNITY_EFFECt";
    }
  }
}
