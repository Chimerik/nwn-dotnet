using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string IlluminationProtectriceEffectTag = "_ILLUMINATION_PROTECTRICE_EFFECT";
    public static void ApplyIlluminationProtectrice(NwCreature caster, NwCreature target)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.IlluminationProtectrice);
      eff.Tag = IlluminationProtectriceEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      target.ApplyEffect(EffectDuration.Permanent, eff);

      if (caster.GetClassInfo(ClassType.Cleric)?.Level > 5)
      {
        target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 6, 2) 
          + CreatureUtils.GetAbilityModifierMin1(caster, Ability.Wisdom))); 
      }
    }
  }
}

