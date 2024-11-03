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

      Effect eff = Effect.DamageImmunityIncrease(resist, 50);
      eff.ShowIcon = false;

      Effect link = Effect.LinkEffects(eff, Effect.Icon(DamageType2da.damageResistanceEffectIcon[resist]));
      link.Tag = ResilienceFielleuseEffectTag;
      link.SubType = EffectSubType.Unyielding;
      link.Creator = caster;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpElementalProtection));

      return link;
    }
  }
}

