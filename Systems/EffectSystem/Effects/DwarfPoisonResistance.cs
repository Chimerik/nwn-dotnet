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
        Effect poisonRes = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        poisonRes.ShowIcon = false;

        Effect eff = Effect.LinkEffects(poisonRes, Effect.Icon(CustomEffectIcon.PoisonResistance));
        eff.Tag = DwarfPoisonResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
