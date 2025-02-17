using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FleauEffectTag = "_FLEAU_EFFECT";
    public static Effect Fleau
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackDecrease);
        eff.Tag = FleauEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Fleau);
        return eff;
      }
    }
  }
}
