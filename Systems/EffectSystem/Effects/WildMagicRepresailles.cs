using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string wildMagicRepresaillesEffectTag = "_EFFECT_WILD_MAGIC_REPRESAILLES";
    public static Effect wildMagicRepresailles
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraOrange), Effect.DamageShield(2, DamageBonus.Plus1d4, DamageType.Magical));
        eff.Tag = wildMagicRepresaillesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
