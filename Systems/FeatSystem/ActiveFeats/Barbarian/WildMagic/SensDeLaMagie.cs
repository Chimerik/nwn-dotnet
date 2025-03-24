using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SensDeLaMagie(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicAwarenessAura, NwTimeSpan.FromRounds(1));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.WildMagicSense);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Sens de la Magie", StringUtils.gold, true, true);
    }
  }
}
