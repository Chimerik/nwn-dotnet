using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BenedictionEffectTag = "_BENEDICTION_EFFECT";
    public static Effect Benediction
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = BenedictionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Benediction);
        return eff;
      }
    }
  }
}
