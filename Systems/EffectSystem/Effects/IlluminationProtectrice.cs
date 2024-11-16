using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string IlluminationProtectriceEffectTag = "_ILLUMINATION_PROTECTRICE_EFFECT";
    public static readonly Native.API.CExoString IlluminationProtectriceEffectExoTag = IlluminationProtectriceEffectTag.ToExoString();
    public static void ApplyIlluminationProtectrice(NwCreature caster, NwCreature target)
    {
      Effect eff = Effect.Icon(EffectIcon.ACIncrease);
      eff.Tag = IlluminationProtectriceEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      target.ApplyEffect(EffectDuration.Permanent, eff);

      if (caster.GetClassInfo(ClassType.Cleric).Level > 5)
      {
        byte modWis = (byte)(caster.GetAbilityModifier(Ability.Wisdom) > 0 ? caster.GetAbilityModifier(Ability.Wisdom) : 1);
        target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 6, 2) + modWis)); 
      }

    }
  }
}

