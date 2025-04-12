using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string WildMagicRepresaillesEffectTag = "_EFFECT_WILD_MAGIC_REPRESAILLES";
    public static Effect wildMagicRepresailles
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraOrange), Effect.Icon(CustomEffectIcon.BarbarianWildMagicRetribution),
          Effect.DamageShield(2, DamageBonus.Plus1d6, DamageType.Magical));
        eff.Tag = WildMagicRepresaillesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
