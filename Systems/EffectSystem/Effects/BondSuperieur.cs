using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BondSuperieurEffectTag = "_BOND_SUPERIEUR_EFFECT";
    public static Effect BondSuperieur
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.BondSuperieur);
        eff.Tag = BondSuperieurEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.BondSuperieur);
        return eff;
      }
    }
  }
}
