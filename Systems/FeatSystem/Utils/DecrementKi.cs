using Anvil.API;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static void DecrementKi(NwCreature creature, byte nbCharge = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPatience, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDelugeDeCoups, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkStunStrike, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDesertion, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkExplosionKi, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPaumeVibratoire, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDarkVision, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkTenebres, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPassageSansTrace, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkSilence, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkFrappeDombre, nbCharge);
    }
  }
}
