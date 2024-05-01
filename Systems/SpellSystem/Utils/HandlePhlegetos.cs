using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void HandlePhlegetos(NwCreature caster, SpellEntry spellEntry)
    {
      if (caster.KnowsFeat((Feat)CustomSkill.FlammesDePhlegetos) && spellEntry.damageType == DamageType.Fire)
      {
        caster.ApplyEffect(EffectDuration.Temporary, Effect.DamageShield(0, DamageBonus.Plus1d4, DamageType.Fire), NwTimeSpan.FromRounds(1));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Flammes de Phlégétos", ColorConstants.Orange, true);
      }
    }
  }
}
