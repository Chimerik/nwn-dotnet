using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MauvaisAugureEffectTag = "_MAUVAIS_AUGURE_EFFECT";
    public static Effect MauvaisAugure
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = MauvaisAugureEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.MauvaisAugure);
        return eff;
      }
    }
  }
}
