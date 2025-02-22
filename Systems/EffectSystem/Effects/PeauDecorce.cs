using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PeauDecorceEffectTag = "_PEAU_DECORCE_EFFECT";

    public static Effect PeauDecorce
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurProtBarkskin);
        eff.Tag = PeauDecorceEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.PeauDecorce);
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
