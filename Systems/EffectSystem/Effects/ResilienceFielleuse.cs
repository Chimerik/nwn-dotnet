using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResilienceFielleuseEffectTag = "_RESILIENCE_FIELLEUSE_EFFECT";
    public static Effect ResilienceFielleuse(NwCreature caster, DamageType resist)
    {
      EffectUtils.RemoveTaggedEffect(caster, ResilienceFielleuseEffectTag);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ResilienceFielleuse);

      var eff = resist switch
      {
        DamageType.Fire => ResistanceFeu,
        DamageType.Cold => ResistanceFroid,
        DamageType.Acid => ResistanceAcide,
        DamageType.Sonic => ResistanceTonnerre,
        DamageType.Electrical => ResistanceElec,
        DamageType.Slashing => ResistanceTranchant,
        DamageType.Piercing => ResistancePercant,
        CustomDamageType.Poison => ResistancePoison,
        CustomDamageType.Necrotic => ResistanceNecrotique,
        CustomDamageType.Psychic => ResistancePsychique,
        _ => ResistanceContondant,
      };

      eff.Tag = ResilienceFielleuseEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpElementalProtection));

      return eff;
    }
  }
}

