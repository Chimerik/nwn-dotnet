using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SleepImmunityEffectTag = "_ELVEN_SLEEP_IMMUNITY_EFFECT";
    public static Effect sleepImmunity
    {
      get
      {
        Effect eff = Effect.Immunity(ImmunityType.Sleep);
        eff.ShowIcon = false;
        eff.Tag = SleepImmunityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
