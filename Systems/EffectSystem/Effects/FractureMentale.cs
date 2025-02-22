using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FractureMentaleEffectTag = "_FACTURE_MENTALE_EFFECT";
    public static Effect FractureMentale
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FractureMentale);
        eff.Tag = FractureMentaleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.FractureMentale);
        return eff;
      }
    }
  }
}
