using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FaveurDivineEffectTag = "_FAVEUR_DIVINE_EFFECT";
    public static Effect FaveurDivine
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageIncrease((int)DamageBonus.Plus1d4, DamageType.Divine));
        eff.Tag = FaveurDivineEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.FaveurDivine);
        return eff;
      }
    }
  }
}
