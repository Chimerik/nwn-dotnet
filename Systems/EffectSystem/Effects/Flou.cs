using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FlouEffectTag = "_FLOU_EFFECT";
    public static Effect Flou
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurGhostlyVisage);
        eff.Tag = FlouEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Flou);
        return eff;
      }
    }
  }
}

