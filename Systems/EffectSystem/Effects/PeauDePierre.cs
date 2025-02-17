using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PeauDePierreEffectTag = "_PEAU_DE_PIERRE_EFFECT";
    public static Effect PeauDePierre
    {
      get
      {
        Effect eff = Effect.LinkEffects(ResistanceTranchant, ResistanceContondant, ResistancePercant, Effect.VisualEffect(VfxType.DurProtStoneskin));
        eff.Tag = PeauDePierreEffectTag;
        eff.Spell = NwSpell.FromSpellType(Spell.Stoneskin);
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
