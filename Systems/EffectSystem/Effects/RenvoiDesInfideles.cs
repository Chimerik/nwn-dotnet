using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RenvoiDesInfidelesEffectTag = "_RENVOI_DES_INFIDELES_EFFECT";

    public static Effect GetRenvoiDesInfidelesEffect(NwCreature target)
    {
      Effect eff = Effect.LinkEffects(Effect.Turned(), Effect.VisualEffect(VfxType.DurMindAffectingFear));
      eff.Tag = RenvoiDesInfidelesEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.OnDamaged -= OnDamageRemoveRenvoi;
      target.OnDamaged += OnDamageRemoveRenvoi;
      return eff;
    }
  }
}
