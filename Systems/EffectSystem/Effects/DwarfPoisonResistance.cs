using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DwarfPoisonResistanceEffectTag = "_DWARF_POISON_RESISTANCE_EFFECT";
    public static Effect DwarfPoisonResistance
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        eff.Tag = DwarfPoisonResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
