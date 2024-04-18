using Anvil.API;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static void DecrementKi(NwCreature creature, byte nbCharge = 1)
    {
      byte remainingKiCharge = (byte)(creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPatience) - nbCharge);

      //await NwTask.NextFrame();

      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPatience, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDelugeDeCoups, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkExplosionKi, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSilence, remainingKiCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, remainingKiCharge);
    }
  }
}
