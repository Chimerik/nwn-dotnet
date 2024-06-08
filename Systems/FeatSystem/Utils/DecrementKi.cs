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

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkDesertion) < 4)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, 0);
        //creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsGuerison, 0);
      }

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre) < 3)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, 0);
      }

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkTenebres) < 2)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSilence, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, 0);
      }
    }
  }
}
