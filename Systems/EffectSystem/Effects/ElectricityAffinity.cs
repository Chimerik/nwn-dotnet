using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ElectricityAffinityEffectTag = "_ELECTRICITY_AFFINITY_EFFECT";
    public static Effect ElectricityAffinity
    {
      get
      {
        Effect resist = Effect.DamageImmunityIncrease(DamageType.Electrical, 50);
        resist.ShowIcon = false;

        Effect eff = Effect.LinkEffects(resist, Effect.Icon(CustomEffectIcon.ElectricalResistance));
        eff.Tag = ElectricityAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
