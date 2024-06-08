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
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraOrange), Effect.DamageShield(2, DamageBonus.Plus1d4, DamageType.Magical));
        eff.Tag = WildMagicRepresaillesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
