using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThieflingPoisonResistanceEffectTag = "_THIEFLING_POISON_RESISTANCE_EFFECT";
    public static Effect ThieflingPoisonResistance
    {
      get
      {
        Effect res = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        res.ShowIcon = false;

        Effect eff = Effect.LinkEffects(res, Effect.Icon(CustomEffectIcon.PoisonResistance));
        eff.Tag = ThieflingPoisonResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
