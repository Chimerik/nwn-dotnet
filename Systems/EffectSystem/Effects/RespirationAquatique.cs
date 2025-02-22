using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RespirationAquatiqueEffectTag = "_RESPIRATION_AQUATIQUE_EFFECT";
    public static Effect RespirationAquatique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.RespirationAquatique), Effect.VisualEffect(VfxType.DurMindAffectingPositive), Effect.RunAction());
        eff.Tag = RespirationAquatiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.RespirationAquatique);
        return eff;
      }
    }
  }
}
