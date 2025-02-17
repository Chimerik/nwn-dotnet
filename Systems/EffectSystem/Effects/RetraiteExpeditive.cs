using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RetraiteExpeditiveEffectTag = "_RETRAITE_EXPEDITIVE_EFFECT";
    public static Effect RetraiteExpeditive
    {
      get
      {
        Effect eff = Effect.VisualEffect(CustomVfx.DashPurple);
        eff.Tag = RetraiteExpeditiveEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.RetraiteExpeditive);
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
