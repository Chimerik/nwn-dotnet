using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FeintTrepasEffectTag = "_FEINT_TREPAS_EFFECT";
    public static Effect FeintTrepas
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.FeintTrepas), Effect.Blindness(), Effect.Knockdown(), Effect.Paralyze(),
          ResistanceAcide, ResistanceContondant, ResistanceElec, ResistanceFeu, ResistanceForce, ResistanceFroid, ResistanceNecrotique, ResistancePercant, ResistancePoison, ResistanceRadiant, ResistanceTonnerre, ResistanceTranchant,
          Effect.Immunity(ImmunityType.Poison));
        eff.Tag = FeintTrepasEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.FeintTrepas);
        return eff;
      }
    }
  }
}
